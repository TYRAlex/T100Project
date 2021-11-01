using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaFramework;
using ILFramework;

public class DoScreenClickEffect : MonoBehaviour
{
    public GameObject effectPrefab;
    public Camera camera;
    bool _isOnEffect = false;
    bool _isOnClick = false;
    public float effectTime = 0.6f;
    GameObjectPool effectPool = null;
    List<GameObject> effects;

    // Start is called before the first frame update
    void Awake()
    {
        effects = new List<GameObject>();
    }

    public void InitPool()
    {
        //Debug.Log("InitPool Effect");
        //effectPool = LuaFramework.LuaHelper.GetObjectPoolManager().CreatePool("Effect", 5, 20, effectPrefab);
        var effect = Instantiate(effectPrefab, new Vector3(10000, 10000, -0.5f), Quaternion.identity, camera.transform.parent);
        Destroy(effect, effectTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && _isOnClick == false)
        {
            _isOnClick = true;
            DoRaycast(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isOnClick = false;
        }
    }

    public void DoRaycast(Vector3 originPosition)
    {
        if (!ILFramework.Util.GetScreenEffectEnable()) return;
        Ray ray = camera.ScreenPointToRay(originPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.DrawLine(ray.origin, hit.point);
            //Debug.Log(hit.point);
            if(hit.collider.name == "ScreenPanel")
            {
                GenEffect(hit.point);
            }
        }
    }

    public void GenEffect(Vector3 position)
    {
        if (_isOnEffect) return;
        //if (effectPool == null) InitPool();
        var effect = Instantiate(effectPrefab, position - new Vector3(0, 0, -0.5f), Quaternion.identity, camera.transform.parent);               
        //var effect = LuaFramework.LuaHelper.GetObjectPoolManager().Get("Effect");
        effects.Add(effect);
        Destroy(effect, effectTime);
        //LuaFramework.LuaHelper.GetSpineManager().DoAnimation(effect, "animation");
        //effect.transform.SetParent(camera.transform.parent);
        //effect.transform.position = position - new Vector3(0, 0, -0.5f);
        StartCoroutine(DelayCheck(0.2f));
        //StartCoroutine(DelayReleaseEffect(effectTime, effect));
    }

    public void DoRelease()
    {
        for(int i = 0; i < effects.Count; i++)
        {
            var go = effects[i];
            DestroyImmediate(go);
            //LuaFramework.LuaHelper.GetObjectPoolManager().Release("Effect", go);
            //go.transform.position = new Vector3(10000f, 10000f, 100f);
            //effects.Remove(go);
        }
    }

    IEnumerator DelayReleaseEffect(float time, GameObject go)
    {
        yield return new WaitForSeconds(time);
        ObjectPoolManager.instance.Release("Effect", go);
        go.transform.position = new Vector3(10000f, 10000f, 100f);
        effects.Remove(go);
    }

    IEnumerator DelayCheck(float time)
    {
        _isOnEffect = true;
        yield return new WaitForSeconds(time);
        _isOnEffect = false;
    }

}
