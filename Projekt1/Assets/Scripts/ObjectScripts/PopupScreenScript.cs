using TMPro;
using UnityEngine;

namespace ObjectScripts
{
    public class PopupScreenScript : MonoBehaviour
    {
        public float ttl = 4;
        private TextMeshProUGUI _textMesh;
        private Color _textColor;
        private float _currentTime = 0;
        void Awake()
        {
            Destroy(gameObject,ttl);
            _textMesh = gameObject.transform.GetComponentInChildren<TextMeshProUGUI>();
            _textColor = _textMesh.color;
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;
            if (_currentTime < ttl / 16)
            {
                _textColor.a = _currentTime / (ttl / 16);
            }
            else if (_currentTime > ttl - ttl / 16)
            {
                _textColor.a = (ttl- _currentTime)/(ttl/16);
            }
        
            _textMesh.color = _textColor;
        }
    }
}

