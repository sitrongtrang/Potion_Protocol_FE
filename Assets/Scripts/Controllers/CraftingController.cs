//using NUnit.Framework;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CraftingController : MonoBehaviour
//{
    

//    public void CollectIngredient(int index)
//    {
//        _Recipe.GetIngredient(index);
//        if (_Recipe.ReadyToCraft()) StartCoroutine(WaitForCraft());
//    }

//    IEnumerator WaitForCraft()
//    {
//        yield return new WaitForSeconds(_Recipe.TimeCrafting);
//        Instantiate(_Recipe.Item, transform.position, Quaternion.identity);
//    }
//}
