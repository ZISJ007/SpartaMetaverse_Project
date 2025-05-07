using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    // ���𰡰� �� ������Ʈ�� �浹���� �� �ڵ����� ȣ���
    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� �̸��� "Rubble"�� ���
        if (collision.gameObject.name.Equals("Rubble"))
        {
            // �ش� ������Ʈ�� ������ ����
            Destroy(collision.gameObject);
        }
    }
}
