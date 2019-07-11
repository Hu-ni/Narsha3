using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CustomMethodSet.conviMethods
{
    public class ConvenientMethods : MonoBehaviour
    {
        // Start is called before the first frame update
        public static GameObject[] GetChildObjectsWithTag(GameObject parentGameObject, string _tag)
        {
            GameObject[] arrayOfChildren = parentGameObject.transform.Cast<Transform>().
                Where(c => c.gameObject.tag == _tag).Select(c => c.gameObject).ToArray();
            return arrayOfChildren;
        }

        public static bool OptionalExistGameObject(List<GameObject> gameObjects, string name) {
            for(int i = 0; i < gameObjects.Count; i++) {
                if (gameObjects[i].name.Equals(name))
                    return true;
            }
            return false;
        }
    }
}


