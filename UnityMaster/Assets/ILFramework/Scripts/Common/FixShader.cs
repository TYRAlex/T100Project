using LuaInterface;
using UnityEngine;
using UnityEngine.UI;

namespace LuaFramework
{
    public class FixShader: MonoBehaviour
    {
        public Shader _shader;
        void Start()
        {
            var image = gameObject.GetComponent<Image>();
            if (image != null)
                gameObject.GetComponent<Image>().material.shader = Shader.Find(_shader.name);
            var render = gameObject.GetComponent<Renderer>();
            if (render != null)
                render.material.shader = Shader.Find(_shader.name);
        }
    }
}