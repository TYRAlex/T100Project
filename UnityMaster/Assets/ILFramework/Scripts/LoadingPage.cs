using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPage : MonoBehaviour
{
    private RawImage _page1;


    private void Awake()
    {
        _page1 = transform.GetRawImage();

        _page1.color = new Color(1, 1, 1, 0.5f);

        Tweener tween1 = _page1.DOFade(1, 0.6f);

        DOTween.Sequence()
            .Append(tween1)
            .AppendInterval(1.0f)
            .AppendInterval(1.0f)
            .AppendInterval(1.0f)
            .OnComplete(()=> {
                SceneManager.LoadSceneAsync("main").completed += OnLoadScene;
            });
           
    }



    private void OnLoadScene(AsyncOperation obj)
    {

    }

}
