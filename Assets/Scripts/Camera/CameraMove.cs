using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private BlockParent blockParent;

    private void Start()
    {
        blockParent.AmountOfRise.Subscribe(value => { transform.position += new Vector3(0, value, 0); })
            .AddTo(gameObject.GetCancellationTokenOnDestroy());
    }
}