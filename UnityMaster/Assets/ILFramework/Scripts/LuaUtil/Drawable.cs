using System.Collections.Generic;
using UnityEngine;
using LuaInterface;
using UnityEngine.EventSystems;

// 1. 将其附加到读/写的sprite图像上
// 2. 将绘图层设置为在光线投射中使用
// 3. 将绘图层设置为在光线投射中使用
// 4. 按住鼠标左键画出这个纹理！
public class Drawable : MonoBehaviour
{
    [Tooltip("彩笔颜色【橡皮擦就是透明彩笔】")]
    public Color Pen_Colour = new Color(1f, 1f, 1f, 0f);
    [Tooltip("钢笔宽度（实际上，它是一个半径，以像素为单位）")]
    public int Pen_Width = 20;
    [Tooltip("图层面板")]
    public LayerMask Drawing_Layers;

    // 在Unity的文件编辑器中必须有读/写权限
    public GameObject[] draw_Obj;
    List<Sprite> drawable_sprite=new List<Sprite>();
    List<Texture2D> drawable_texture=new List<Texture2D>();


    Vector2 previous_drag_position;
    List<Color[]> clean_colours_array=new List<Color[]>();
    Color transparent;
    Color32[] cur_colors;
    bool mouse_was_previously_held_down = false;
    bool no_drawing_on_current_drag = false;

    Color[] testcolors;

    public int currentIndex;
    public int showIndex;
    public LuaFunction OnDrawableLua;

    void Awake()
    {
        Debug.Log(draw_Obj.Length + "+++++++");
        for (int i = 0; i < draw_Obj.Length; i++)
        {
            drawable_sprite.Add(draw_Obj[i].transform.GetComponent<SpriteRenderer>().sprite);
            drawable_texture.Add(drawable_sprite[i].texture);
            clean_colours_array.Add(drawable_texture[i].GetPixels(0, 0, drawable_texture[i].width, drawable_texture[i].height));
        }
        // 保存图片所有颜色
        //Debug.Log("开始颜色length" + clean_colours_array.Length.ToString());
        //Debug.Log(Pen_Colour);
    }


    void Update()
    {
        bool mouse_held_down = Input.GetMouseButton(0);
        if (mouse_held_down && !no_drawing_on_current_drag)
        {
            // 将鼠标坐标转换为世界坐标
            Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 检查当前鼠标位置是否与我们的图像重叠
            Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
            if (hit != null && hit.transform != null)
            // 我们已经超越了我们所描绘的纹理！改变像素的颜色
            {
                switch (hit.name)
                {
                    case "ReadWritePng1":
                        currentIndex = 0;
                        showIndex = 4;
                        break;
                    case "ReadWritePng2":
                        currentIndex = 1;
                        showIndex = 1;
                        break;
                    case "ReadWritePng3":
                        currentIndex = 2;
                        showIndex = 2;
                        break;
                    case "ReadWritePng4":
                        currentIndex = 3;
                        showIndex = 3;
                        break;
                    default:
                        break;
                }
                ChangeColourAtPoint(mouse_world_position);            
            }
            else
            {
                currentIndex = -1;
                showIndex = -1;
                hit = null;
                // 我们还没有结束我们的目标纹理
                previous_drag_position = Vector2.zero;
                if (!mouse_was_previously_held_down)
                {
                    //这是一个新的拖动用户在画布上点击离开
                    //在新的阻力开始之前，确保不会发生绘图
                    no_drawing_on_current_drag = true;
                }
            }
        }
        // 鼠标释放
        else if (!mouse_held_down)
        {
            previous_drag_position = Vector2.zero;
            no_drawing_on_current_drag = false;
        }
        mouse_was_previously_held_down = mouse_held_down;
    }

    //获取某一像素点的个数
    private int GetAllPoint()
    {
        int pointCount = 0;
        for (int i = 0; i < clean_colours_array[currentIndex].Length; i++)
        {
            if (testcolors[i] == clean_colours_array[currentIndex][i])
            {
                pointCount++;
            }
        }
        return pointCount;
    }

    // 在世界坐标中传递一个点
    // 改变世界周围的像素点指向静态的钢笔颜色
    public void ChangeColourAtPoint(Vector2 world_point)
    {
        // 改变坐标到这个图像的局部坐标
        Vector3 local_pos = draw_Obj[currentIndex].transform.InverseTransformPoint(world_point);

        // 将这些转换为像素坐标
        float pixelWidth = drawable_sprite[currentIndex].rect.width;
        float pixelHeight = drawable_sprite[currentIndex].rect.height;
        float unitsToPixels = pixelWidth / drawable_sprite[currentIndex].bounds.size.x * draw_Obj[currentIndex].transform.localScale.x;

        // 需要把我们的坐标居中
        float centered_x = local_pos.x * unitsToPixels + pixelWidth / 2;
        float centered_y = local_pos.y * unitsToPixels + pixelHeight / 2;

        // 将鼠标移动到最近的像素点
        Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

        cur_colors = drawable_texture[currentIndex].GetPixels32();

        if (previous_drag_position == Vector2.zero)
        {
            // 如果这是我们第一次拖动这个图像，只需在鼠标位置上着色像素
            MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
        }
        else
        {
            // 从我们上次更新呼叫的位置上的颜色
            ColourBetween(previous_drag_position, pixel_pos);
        }
        ApplyMarkedPixelChanges();

        previous_drag_position = pixel_pos;
    }


    // 从起始点一直到终点，将像素的颜色设置为直线，以确保中间的所有内容都是彩色的        
    public void ColourBetween(Vector2 start_point, Vector2 end_point)
    {
        // 从开始到结束的距离
        float distance = Vector2.Distance(start_point, end_point);
        Vector2 direction = (start_point - end_point).normalized;

        Vector2 cur_position = start_point;

        // 计算在开始点和结束点之间插入多少次，基于上次更新以来的时间量
        float lerp_steps = 1 / distance;

        for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
        {
            cur_position = Vector2.Lerp(start_point, end_point, lerp);
            MarkPixelsToColour(cur_position, Pen_Width, Pen_Colour);
        }
    }


    public void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
    {
        // 算出我们需要在每个方向上着色多少个像素（x和y）
        int center_x = (int)center_pixel.x;
        int center_y = (int)center_pixel.y;
        int extra_radius = Mathf.Min(0, pen_thickness - 2);
        for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
        {
            // 检查X是否包围了图像，所以我们不会在图像的另一侧画像素
            if (x >= (int)drawable_sprite[currentIndex].rect.width
                || x < 0)
                continue;

            for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
            {
                MarkPixelToChange(x, y, color_of_pen);
            }
        }
    }
    public void MarkPixelToChange(int x, int y, Color color)
    {
        // 需要将x和y坐标转换为数组的平面坐标
        int array_pos = y * (int)drawable_sprite[currentIndex].rect.width + x;

        // 检查这是否是一个有效的位置
        if (array_pos > cur_colors.Length || array_pos < 0)
            return;

        cur_colors[array_pos] = color;
    }
    public void ApplyMarkedPixelChanges()
    {
        Color[] testTempcolors;
        drawable_texture[currentIndex].SetPixels32(cur_colors);
        drawable_texture[currentIndex].Apply();
        testTempcolors = drawable_texture[currentIndex].GetPixels(0, 0, drawable_texture[currentIndex].width, drawable_texture[currentIndex].height);
        testcolors = testTempcolors;
        //判断下当下刮的像素点的个数
        int count = GetAllPoint();
        //Debug.Log("count" + count.ToString());
        if (count <= (int)clean_colours_array[currentIndex].Length / 2.5)
        {
            SetAllPixel();
            //Debug.Log("gua wan le "+currentIndex.ToString());     
        }
    }


    // 直接颜色像素。这个方法比使用MarkPixelsToColour要慢，然后使用applymarkepixelchanges
    // SetPixels32比SetPixel快得多
    // 根据笔的厚度（笔的半径），以中心像素为中心像素，并以像素为中心像素。
    public void ColourPixels(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
    {
        // 算出我们需要在每个方向上着色多少个像素（x和y）
        int center_x = (int)center_pixel.x;
        int center_y = (int)center_pixel.y;
        int extra_radius = Mathf.Min(0, pen_thickness - 2);

        for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
        {
            for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
            {
                drawable_texture[currentIndex].SetPixel(x, y, color_of_pen);
            }
        }

        drawable_texture[currentIndex].Apply();
    }

    private void SetAllPixel()
    {
        for (int i = 0; i < cur_colors.Length; i++)
        {
            cur_colors[i] = Pen_Colour;
        }
        drawable_texture[currentIndex].SetPixels32(cur_colors);
        drawable_texture[currentIndex].Apply();
        //gameObject.SetActive(false);
        //全部显示将当期的索引传lua进行判断
        if (OnDrawableLua != null) OnDrawableLua.Call(currentIndex,showIndex);
    }
    private void OnEnable()
    {
        //还原被擦除的图片颜色
        Debug.Log(draw_Obj.Length + "测111"+ drawable_texture.Count);

        if (drawable_texture != null)
        {
            for (int i = 0; i < drawable_texture.Count; i++)
            {        
                drawable_texture[i].SetPixels(clean_colours_array[i]);
                drawable_texture[i].Apply();
            }
        }
    }
    private void OnDestroy()
    {
        //还原被擦除的图片颜色
        for (int i = 0; i < drawable_texture.Count; i++)
        {
            drawable_texture[i].SetPixels(clean_colours_array[i]);
            drawable_texture[i].Apply();
        }
    }

    
}
