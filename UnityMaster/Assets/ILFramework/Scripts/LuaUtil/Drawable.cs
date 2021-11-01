using System.Collections.Generic;
using UnityEngine;
using LuaInterface;
using UnityEngine.EventSystems;

// 1. ���丽�ӵ���/д��spriteͼ����
// 2. ����ͼ������Ϊ�ڹ���Ͷ����ʹ��
// 3. ����ͼ������Ϊ�ڹ���Ͷ����ʹ��
// 4. ��ס�����������������
public class Drawable : MonoBehaviour
{
    [Tooltip("�ʱ���ɫ����Ƥ������͸���ʱʡ�")]
    public Color Pen_Colour = new Color(1f, 1f, 1f, 0f);
    [Tooltip("�ֱʿ�ȣ�ʵ���ϣ�����һ���뾶��������Ϊ��λ��")]
    public int Pen_Width = 20;
    [Tooltip("ͼ�����")]
    public LayerMask Drawing_Layers;

    // ��Unity���ļ��༭���б����ж�/дȨ��
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
        // ����ͼƬ������ɫ
        //Debug.Log("��ʼ��ɫlength" + clean_colours_array.Length.ToString());
        //Debug.Log(Pen_Colour);
    }


    void Update()
    {
        bool mouse_held_down = Input.GetMouseButton(0);
        if (mouse_held_down && !no_drawing_on_current_drag)
        {
            // ���������ת��Ϊ��������
            Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // ��鵱ǰ���λ���Ƿ������ǵ�ͼ���ص�
            Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
            if (hit != null && hit.transform != null)
            // �����Ѿ���Խ�����������������ı����ص���ɫ
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
                // ���ǻ�û�н������ǵ�Ŀ������
                previous_drag_position = Vector2.zero;
                if (!mouse_was_previously_held_down)
                {
                    //����һ���µ��϶��û��ڻ����ϵ���뿪
                    //���µ�������ʼ֮ǰ��ȷ�����ᷢ����ͼ
                    no_drawing_on_current_drag = true;
                }
            }
        }
        // ����ͷ�
        else if (!mouse_held_down)
        {
            previous_drag_position = Vector2.zero;
            no_drawing_on_current_drag = false;
        }
        mouse_was_previously_held_down = mouse_held_down;
    }

    //��ȡĳһ���ص�ĸ���
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

    // �����������д���һ����
    // �ı�������Χ�����ص�ָ��̬�ĸֱ���ɫ
    public void ChangeColourAtPoint(Vector2 world_point)
    {
        // �ı����굽���ͼ��ľֲ�����
        Vector3 local_pos = draw_Obj[currentIndex].transform.InverseTransformPoint(world_point);

        // ����Щת��Ϊ��������
        float pixelWidth = drawable_sprite[currentIndex].rect.width;
        float pixelHeight = drawable_sprite[currentIndex].rect.height;
        float unitsToPixels = pixelWidth / drawable_sprite[currentIndex].bounds.size.x * draw_Obj[currentIndex].transform.localScale.x;

        // ��Ҫ�����ǵ��������
        float centered_x = local_pos.x * unitsToPixels + pixelWidth / 2;
        float centered_y = local_pos.y * unitsToPixels + pixelHeight / 2;

        // ������ƶ�����������ص�
        Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

        cur_colors = drawable_texture[currentIndex].GetPixels32();

        if (previous_drag_position == Vector2.zero)
        {
            // ����������ǵ�һ���϶����ͼ��ֻ�������λ������ɫ����
            MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
        }
        else
        {
            // �������ϴθ��º��е�λ���ϵ���ɫ
            ColourBetween(previous_drag_position, pixel_pos);
        }
        ApplyMarkedPixelChanges();

        previous_drag_position = pixel_pos;
    }


    // ����ʼ��һֱ���յ㣬�����ص���ɫ����Ϊֱ�ߣ���ȷ���м���������ݶ��ǲ�ɫ��        
    public void ColourBetween(Vector2 start_point, Vector2 end_point)
    {
        // �ӿ�ʼ�������ľ���
        float distance = Vector2.Distance(start_point, end_point);
        Vector2 direction = (start_point - end_point).normalized;

        Vector2 cur_position = start_point;

        // �����ڿ�ʼ��ͽ�����֮�������ٴΣ������ϴθ���������ʱ����
        float lerp_steps = 1 / distance;

        for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
        {
            cur_position = Vector2.Lerp(start_point, end_point, lerp);
            MarkPixelsToColour(cur_position, Pen_Width, Pen_Colour);
        }
    }


    public void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
    {
        // ���������Ҫ��ÿ����������ɫ���ٸ����أ�x��y��
        int center_x = (int)center_pixel.x;
        int center_y = (int)center_pixel.y;
        int extra_radius = Mathf.Min(0, pen_thickness - 2);
        for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
        {
            // ���X�Ƿ��Χ��ͼ���������ǲ�����ͼ�����һ�໭����
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
        // ��Ҫ��x��y����ת��Ϊ�����ƽ������
        int array_pos = y * (int)drawable_sprite[currentIndex].rect.width + x;

        // ������Ƿ���һ����Ч��λ��
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
        //�ж��µ��¹ε����ص�ĸ���
        int count = GetAllPoint();
        //Debug.Log("count" + count.ToString());
        if (count <= (int)clean_colours_array[currentIndex].Length / 2.5)
        {
            SetAllPixel();
            //Debug.Log("gua wan le "+currentIndex.ToString());     
        }
    }


    // ֱ����ɫ���ء����������ʹ��MarkPixelsToColourҪ����Ȼ��ʹ��applymarkepixelchanges
    // SetPixels32��SetPixel��ö�
    // ���ݱʵĺ�ȣ��ʵİ뾶��������������Ϊ�������أ���������Ϊ�������ء�
    public void ColourPixels(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
    {
        // ���������Ҫ��ÿ����������ɫ���ٸ����أ�x��y��
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
        //ȫ����ʾ�����ڵ�������lua�����ж�
        if (OnDrawableLua != null) OnDrawableLua.Call(currentIndex,showIndex);
    }
    private void OnEnable()
    {
        //��ԭ��������ͼƬ��ɫ
        Debug.Log(draw_Obj.Length + "��111"+ drawable_texture.Count);

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
        //��ԭ��������ͼƬ��ɫ
        for (int i = 0; i < drawable_texture.Count; i++)
        {
            drawable_texture[i].SetPixels(clean_colours_array[i]);
            drawable_texture[i].Apply();
        }
    }

    
}
