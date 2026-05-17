using UnityEngine;

// 放在關卡入口的觸發器。皮老闆通過後，死掉就會重生在這一關的起點，
// 而不是回到整個遊戲最一開始的出生點。
public class Checkpoint : MonoBehaviour
{
    [Header("重生點")]
    // 死掉要回到的位置。沒指定就用這個 Checkpoint 自己的位置。
    public Transform respawnPoint;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        Plankton plankton = other.GetComponent<Plankton>();
        if (plankton == null) return;

        triggered = true;

        Transform target = respawnPoint != null ? respawnPoint : transform;
        plankton.SetCheckpoint(target.position, target.rotation);
    }
}
