using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// 位置偏移量
    /// </summary>
    private Vector3 _offset = Vector3.zero;

    /// <summary>
    /// 初始位置
    /// </summary>
    private Vector3 _startPos;

    /// <summary>
    /// 拖拽ing返回的是localPosition
    /// </summary>
    private Action<Vector3> _draging = null;

    /// <summary>
    /// 拖拽结束返回的是localPosition
    /// </summary>
    private Action<Vector3, bool> _dragEnd = null;

    /// <summary>
    /// 修正拖拽区域的回调
    /// </summary>
    public Action RepairCurContentCallBack = null;

    /// <summary>
    /// 拖拽限制范围
    /// </summary>
    public RectTransform DragRect;

    /// <summary>
    /// 成功范围
    /// </summary>
    public RectTransform SucceedEndRect;


    private float _minX;
    private float _maxX;
    private float _minY;
    private float _maxY;


    private RectTransform _rT;

    private bool _isMatch = false;

    public List<RectTransform> GameObjectRectTransforms;

    private Dictionary<string, Vector3[]> _vertexPosDics;
    private Vector3[] _myVertexPos;


    public float WideHalf;
    public float HighHalf;

    public Vector3[] MyVertexPos => _myVertexPos;
    public Dictionary<string, Vector3[]> VertexPosDics => _vertexPosDics;

    private List<Vector3[]> _curContentV3;  //当前拖拽区域的坐标
    public RectTransform TempContent;  //临时拖拽区域
    private Vector3 _lastPos;  //上一帧的位置

    private void Awake()
    {
        _rT = GetComponent<RectTransform>();
        _startPos = transform.localPosition;
        _lastPos = transform.localPosition;

        InitData();
    }

    public void InitData()
    {

       

        _vertexPosDics = new Dictionary<string, Vector3[]>();
        _myVertexPos = new Vector3[4];
        _curContentV3 = new List<Vector3[]>();

        InitMyVerterPos();
        InitVertexPosDics();

        WideHalf = _rT.rect.width / 2;
        HighHalf = _rT.rect.height / 2;
    }

    /// <summary>
    /// 初始化自身顶点坐标
    /// </summary>
    private void InitMyVerterPos()
    {
        var pos = GetVertexLocalPosition(_rT);
        for (int i = 0; i < pos.Length; i++)
            _myVertexPos[i] = pos[i];
    }

    /// <summary>
    /// 初始化拖拽范围顶点坐标
    /// </summary>
    private void InitVertexPosDics()
    {
        if (GameObjectRectTransforms.Count == 0)
        {
            Debug.LogError("请先添加GameObjectRectTransforms，_vertexPosDics初始化失败");
            return;
        }

        foreach (var item in GameObjectRectTransforms)
            _vertexPosDics.Add(item.name, GetVertexLocalPosition(item));
    }


    /// <summary>
    /// 更新_curContentV3数据
    /// </summary>
    /// <param name="mPos"></param>
    private void UpdateCurContentV3(Vector3[] mPos)
    {
        var mXMin = Math.Floor(mPos[0].x); var mYMin = Math.Floor(mPos[0].y);
        var mXMax = Math.Floor(mPos[2].x); var mYMax = Math.Floor(mPos[2].y);

        for (int i = 0; i < _curContentV3.Count; i++)
        {
            var item = _curContentV3[i];


            var oXMin = Math.Floor(item[0].x); var oYMin = Math.Floor(item[0].y);
            var oXMax = Math.Floor(item[2].x); var oYMax = Math.Floor(item[2].y);

            bool isXRange = (oXMin <= mXMin) && (mXMax <= oXMax);
            bool isYRange = (oYMin <= mYMin) && (mYMax <= oYMax);

            if (!isXRange || !isYRange)
            {
                _curContentV3.Remove(item);
                i--;
            }
        }
    }

    /// <summary>
    /// 设置_curContentV3 数据
    /// </summary>
    /// <param name="mPos"></param>
    private void SetCurContentV3(Vector3[] mPos)
    {
        var mXMin = Math.Floor(mPos[0].x); var mYMin = Math.Floor(mPos[0].y);
        var mXMax = Math.Floor(mPos[2].x); var mYMax = Math.Floor(mPos[2].y);

        foreach (var item in _vertexPosDics)
        {
            var key = item.Key;
            var value = item.Value;

            var oXMin = Math.Floor(value[0].x); var oYMin = Math.Floor(value[0].y);
            var oXMax = Math.Floor(value[2].x); var oYMax = Math.Floor(value[2].y);

            bool isXRange = (oXMin <= mXMin) && (mXMax <= oXMax);
            bool isYRange = (oYMin <= mYMin) && (mYMax <= oYMax);

            if (isXRange && isYRange)
            {
                if (!_curContentV3.Contains(_vertexPosDics[key]))
                { _curContentV3.Add(_vertexPosDics[key]); }
            }
        }

        UpdateCurContentV3(mPos);

        // Debug.LogError("CurContent.Count:" + _curContentV3.Count);


        if (_curContentV3.Count == 1)
            SetCurContent();
        else if (_curContentV3.Count > 1)
            SetTempContent();
        else if (_curContentV3.Count == 0)
            RepairCurContent();
    }

    private Vector3 _tempPos = new Vector3(0, 0, 0);     //临时拖拽区域Pos
    private Vector2 _tempSize = new Vector2(100, 100);   //临时拖拽区域Size

    private List<float> _tempX = new List<float>();      //临时X坐标值
    private List<float> _tempY = new List<float>();      //临时Y坐标值

    /// <summary>
    /// 设置临时拖拽区域
    /// </summary>
    private void SetTempContent()
    {
        if (_curContentV3.Count > 2)
        {
            RepairCurContentCallBack?.Invoke();
        }

        _tempX.Clear(); _tempY.Clear();


        foreach (var item in _curContentV3)
        {
            var leftDownPos = item[0];
            var rightUpPos = item[2];

            _tempX.Add(leftDownPos.x);
            _tempX.Add(rightUpPos.x);

            _tempY.Add(leftDownPos.y);
            _tempY.Add(rightUpPos.y);

        }

        _tempX.Sort(); _tempY.Sort();

        var xMin = _tempX[0]; var xMax = _tempX[_tempX.Count - 1];
        var yMin = _tempY[0]; var yMax = _tempY[_tempY.Count - 1];


        _tempPos.x = (xMin + xMax) / 2;
        _tempPos.y = (yMin + yMax) / 2;

        var width = xMin - xMax; var hight = yMin - yMax;

        if (width < 0)
            width = -width;

        if (hight < 0)
            hight = -hight;

        _tempSize.x = width;
        _tempSize.y = hight;

        TempContent.localPosition = _tempPos;
        TempContent.sizeDelta = _tempSize;

        DragRect = TempContent;

    }

    /// <summary>
    /// 重置临时拖拽区域
    /// </summary>
    private void DoResetTempConten()
    {
        _tempSize.x = 100;
        _tempSize.y = 100;

        _tempPos.x = 0;
        _tempPos.y = -468;

        TempContent.localPosition = _tempPos;
        TempContent.sizeDelta = _tempSize;

    }

    /// <summary>
    /// 设置当前拖拽区域
    /// </summary>
    private void SetCurContent()
    {
        DoResetTempConten();
        var curContentV3 = _curContentV3[0];
        foreach (var item in _vertexPosDics)
        {
            if (curContentV3 == item.Value)
            {
                foreach (var rect in GameObjectRectTransforms)
                {
                    if (item.Key == rect.gameObject.name)
                    {
                        DragRect = rect;
                        break;
                    }
                }
                break;
            }
        }
    }

    /// <summary>
    /// 修复当前拖拽区域
    /// </summary>
    private void RepairCurContent()
    {
        _rT.localPosition = _lastPos;

    }

    /// <summary>
    /// 设置拖拽最大值和最小值
    /// </summary>
    private void SetDragRange()
    {
        if (DragRect == null)
        {
            Debug.LogError("DragRect is Null...");
            return;
        }

        var position = DragRect.transform.localPosition;

        _minX = position.x - DragRect.pivot.x * DragRect.rect.width + _rT.rect.width * _rT.pivot.x;
        _maxX = position.x + (1 - DragRect.pivot.x) * DragRect.rect.width - _rT.rect.width * (1 - _rT.pivot.x);
        _minY = position.y - DragRect.pivot.y * DragRect.rect.height + _rT.rect.height * _rT.pivot.y;
        _maxY = position.y + (1 - DragRect.pivot.y) * DragRect.rect.height - _rT.rect.height * (1 - _rT.pivot.y);

    }

    /// <summary>
    /// 限制坐标范围
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Vector3 DragRangeLimit(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, _minX, _maxX);
        pos.y = Mathf.Clamp(pos.y, _minY, _maxY);
        return pos;
    }

    /// <summary>
    /// 重置自身位置
    /// </summary>
    public void DoReset()
    {
        transform.localPosition = _startPos;
        DragRect = GameObjectRectTransforms[0];
    }

    /// <summary>
    /// 获取UI局部顶点坐标
    /// 顺序：左下、左上、右上、右下，
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <returns></returns>
    private Vector3[] GetVertexLocalPosition(RectTransform rectTransform)
    {
        Vector3[] vertexPos = new Vector3[4];

        var rect = rectTransform.rect;

        var localPosition = rectTransform.localPosition;
        var localPosX = localPosition.x;
        var localPosY = localPosition.y;

        var xMin = rect.xMin; var yMin = rect.yMin;
        var xMan = rect.xMax; var yMax = rect.yMax;

        Vector3 leftDown = new Vector3(localPosX + xMin, localPosY + yMin, 0);
        Vector3 leftUp = new Vector3(localPosX + xMin, localPosY + yMax, 0);
        Vector3 rightUp = new Vector3(localPosX + xMan, localPosY + yMax, 0);
        Vector3 rightDown = new Vector3(localPosX + xMan, localPosY + yMin, 0);

        vertexPos[0] = leftDown;
        vertexPos[1] = leftUp;
        vertexPos[2] = rightUp;
        vertexPos[3] = rightDown;

        return vertexPos;
    }

    /// <summary>
    /// 获取My最新的顶点坐标
    ///  顺序：左下、左上、右上、右下
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <returns></returns>
    private Vector3[] GetNewestMyVertexLocalPosition(RectTransform rectTransform)
    {
        var rect = rectTransform.rect;

        var localPosition = rectTransform.localPosition;
        var localPosX = localPosition.x;
        var localPosY = localPosition.y;

        var xMin = rect.xMin; var yMin = rect.yMin;
        var xMan = rect.xMax; var yMax = rect.yMax;

        _myVertexPos[0].x = localPosX + xMin;
        _myVertexPos[0].y = localPosY + yMin;

        _myVertexPos[1].x = localPosX + xMin;
        _myVertexPos[1].y = localPosY + yMax;

        _myVertexPos[2].x = localPosX + xMan;
        _myVertexPos[2].y = localPosY + yMax;

        _myVertexPos[3].x = localPosX + xMan;
        _myVertexPos[3].y = localPosY + yMin;

        return _myVertexPos;
    }




    #region 接口
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rT, eventData.position, eventData.enterEventCamera, out Vector3 globalMousePos))
            _offset = _rT.position - globalMousePos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetDragRange();

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rT, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePos))
        {
            _rT.position = globalMousePos + _offset;
            _rT.localPosition = DragRangeLimit(_rT.localPosition);
            var mVertexPos = GetNewestMyVertexLocalPosition(_rT);
            SetCurContentV3(mVertexPos);
        }
        _lastPos = _rT.localPosition;

        _draging?.Invoke(new Vector2(transform.localPosition.x, transform.localPosition.y));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isMatch = false;
        if (RectTransformUtility.RectangleContainsScreenPoint(SucceedEndRect, this.transform.position, eventData.pressEventCamera))
            _isMatch = true;
        _dragEnd?.Invoke(new Vector2(transform.localPosition.x, transform.localPosition.y), _isMatch);
    }
    #endregion

    public void SetMapDragCallBack(Action<Vector3> draging, Action<Vector3, bool> dragEnd)
    {
        _draging = draging;
        _dragEnd = dragEnd;
    }
}



