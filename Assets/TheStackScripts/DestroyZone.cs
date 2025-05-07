using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    // 무언가가 이 오브젝트와 충돌했을 때 자동으로 호출됨
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트의 이름이 "Rubble"인 경우
        if (collision.gameObject.name.Equals("Rubble"))
        {
            // 해당 오브젝트를 씬에서 제거
            Destroy(collision.gameObject);
        }
    }
}
