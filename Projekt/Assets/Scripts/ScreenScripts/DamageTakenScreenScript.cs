using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ObjectScripts
{
    public class DamageTakenScreenScript : MonoBehaviour
    {
        public float ttl = 4;
        private Image _image;
        private Color _imageColor;
        private float _currentTime = 0;
        void Awake()
        {
            Destroy(gameObject,ttl);
            _image = gameObject.transform.GetComponentInChildren<Image>();
            _imageColor = _image.color;
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;
            if (_currentTime < ttl / 4)
            {
                _imageColor.a = _currentTime / (ttl / 4);
            }
            else if (_currentTime > ttl - ttl / 4)
            {
                _imageColor.a = (ttl- _currentTime)/(ttl/4);
            }
        
            _image.color = _imageColor;
        }
    }
}

