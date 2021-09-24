using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers{
    //Classe generica per la generazione di un pool di gameobjects
    [System.Serializable]
    public class Pool 
    {
        public string poolName;
        public List<GameObject> poolList;
        public GameObject prefab;
        //public Transform container;
        public int listSize;
    }
}
