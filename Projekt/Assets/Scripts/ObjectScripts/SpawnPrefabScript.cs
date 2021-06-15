using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ObjectScripts
{
    public class SpawnPrefabScript : MonoBehaviour
    {
        public GameObject prefab;
        public float spawnEveryXSeconds;
        [Range(0,1)]
        public float randomRange;
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
            }
        }

        private void SpawnPrefab()
        {
            _tts = ((Random.value - 0.5f)*randomRange+1)*spawnEveryXSeconds; //Bei randomRange = 0.2 == [-0.2, 0.2] von spawnZeit wird zufällig hinzugefügt, also [0.8,1.2] von SpawnZeit
            Transform tf = transform; //Geht an sich auch 1-Mal zu start, so kann aber der Spawnpunkt bewegt werden
            Instantiate(prefab, tf.position, tf.rotation, tf);
        }
    }
}
