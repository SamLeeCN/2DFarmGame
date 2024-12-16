using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
using UnityEngine.Pool;
public class PoolManager : Singleton<PoolManager>
{
    
    public List<GameObject> poolPrefabs = new List<GameObject>();
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();
    private Queue<GameObject> soundEffectQueue = new Queue<GameObject>();

    private void OnEnable()
    {
        EventHandler.ParticleEffectEvent += OnParticleEffectEvent;
        
    }

    private void OnDisable()
    {
        EventHandler.ParticleEffectEvent -= OnParticleEffectEvent;
        
    }

    

    private void Start()
    {
        CreatePool();
    }

    private void CreatePool()
    {
        foreach (GameObject prefab in poolPrefabs)
        {
            Transform parent = new GameObject(prefab.name).transform;
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                () => Instantiate(prefab, parent),
                e => { e.SetActive(true); },
                e => { e.SetActive(false); },
                e => { Destroy(e); }
            );

            poolEffectList.Add(newPool);
        }
    }

    private void OnParticleEffectEvent(ParticleEffectType effectType, Vector3 pos)
    {
        ObjectPool<GameObject> objPool = effectType switch
        { 
            ParticleEffectType.Tree01LeavesFalling => poolEffectList[0],
            ParticleEffectType.Tree02LeavesFalling => poolEffectList[1],
            ParticleEffectType.RockBreak => poolEffectList[2],
            _ => null,
        };
        GameObject obj = objPool.Get();
        obj.transform.position = pos;
        StartCoroutine(ReleaseParticleCoroutine(objPool, obj, 1.5f));
    }

    private IEnumerator ReleaseParticleCoroutine(ObjectPool<GameObject> objPool, GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        objPool.Release(obj);
    }

    /*private void OnSoundEffectEvent(SoundDetails soundDetails, Vector3 pos)
    {
        ObjectPool<GameObject> pool = poolEffectList[3];

        var obj = pool.Get();
        obj.transform.position = pos;
        obj.GetComponent<Sound>().SetSound(soundDetails);
        StartCoroutine(ReleaseSoundCoroutine(pool, obj, soundDetails));
    }

    private IEnumerator ReleaseSoundCoroutine(ObjectPool<GameObject> pool, GameObject obj, SoundDetails soundDetails)
    {
        yield return new WaitForSeconds(soundDetails.soundClip.length);
        pool.Release(obj);
    }*/

    public void InitSoundEffect(SoundDetails soundDetails, Vector3 pos)
    {
        var obj = GetSoundEffectObject();
        obj.transform.position = pos;
        obj.GetComponent<Sound>().SetSound(soundDetails);
        obj.SetActive(true);
        StartCoroutine(ReleaseSoundCoroutine(obj, soundDetails));
    }

    private void CreateSoundEffectPool()
    {
        var parent = new GameObject(poolPrefabs[3].name).transform;
        parent.SetParent(transform);

        for (int i = 0; i < 20; i++)
        {
            var newObj = Instantiate(poolPrefabs[3], parent);
            newObj.SetActive(false);
            soundEffectQueue.Enqueue(newObj);
        }
    }

    private GameObject GetSoundEffectObject()
    {
        if (soundEffectQueue.Count < 2)
        {
            CreateSoundEffectPool();
        }
        return soundEffectQueue.Dequeue();
    }

    

    private IEnumerator ReleaseSoundCoroutine(GameObject obj, SoundDetails soundDetails)
    {
        yield return new WaitForSeconds(soundDetails.soundClip.length);
        obj.SetActive(false);
        soundEffectQueue.Enqueue(obj);
    }
}
