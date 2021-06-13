using System;
using UnityEngine;

namespace ObjectScripts
{
    public class SpawnPrefabScript : MonoBehaviour
    {
        public GameObject prefab;
        public float spawnEveryXSeconds;
        private float _tts; //Time To Spawn
        // Update is called once per frame
        private void Start()
        {
            _tts = spawnEveryXSeconds;
        }

        private void Update()
        {
            _tts -= Time.deltaTime;
            if (_tts < 0)
            {
                SpawnPrefab();
                _tts = spawnEveryXSeconds;
            }
        }

        private void SpawnPrefab()
        {
            Transform tf = transform; //Geht an sich auch 1-Mal zu start, so kann aber der Spawnpunkt bewegt werden
            Instantiate(prefab, tf.position, tf.rotation, tf);
        }
    }
}
