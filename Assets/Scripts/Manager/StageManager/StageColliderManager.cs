using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class StageColliderManager : MonoBehaviour
{
    [SerializeField] private BoxCollider2D wallLeft;
    [SerializeField] private BoxCollider2D wallRight;
    [SerializeField] private BoxCollider2D wallLow;
    [SerializeField] private BlockParent blockParent;
    private int _count;
    private const float DefaultValue = 0;
    private const float Offset = 1f;

    public void Initialize()
    {
        _count = 0;
        SetupCollider(DefaultValue);
        blockParent.AmountOfRise.ThrottleFirst(TimeSpan.FromSeconds(1)).Subscribe(SetupCollider)
            .AddTo(gameObject.GetCancellationTokenOnDestroy());
    }

    private void SetupCollider(float amountOfRise)
    {
        var width = Screen.width;
        var height = Screen.height;
        if (Math.Abs(amountOfRise - DefaultValue) > 0)
        {
            _count++;
            Debug.Log(_count);
            if (Camera.main != null)
            {
                var rightTop = Camera.main.ScreenToWorldPoint(new Vector3(width, height, 0));
                Camera main;
                var leftBottom = (main = Camera.main).ScreenToWorldPoint(new Vector3(0, 0, 0));
                var position = main.transform.position;
                wallLow.offset = new Vector2(0, leftBottom.y - 0.5f - Offset);
                wallLow.size = new Vector2(rightTop.x * 5, 1);
                wallLeft.offset = new Vector2(leftBottom.x - 0.5f - Offset, position.y);
                wallLeft.size = new Vector2(1, (rightTop.y * 2) - (amountOfRise * 2 * _count) + (Offset * 2));
                wallRight.offset = new Vector2(rightTop.x + 0.5f + Offset, position.y);
                wallRight.size = new Vector2(1, (rightTop.y * 2) - (amountOfRise * 2 * _count) + (Offset * 2));
            }
        }
        else
        {
            if (Camera.main != null)
            {
                var rightTop = Camera.main.ScreenToWorldPoint(new Vector3(width, height, 0));
                Camera main;
                var leftBottom = (main = Camera.main).ScreenToWorldPoint(new Vector3(0, 0, 0));
                var position = main.transform.position;
                wallLow.offset = new Vector2(0, leftBottom.y - 0.5f - Offset);
                wallLow.size = new Vector2(rightTop.x * 5, 1);
                wallLeft.offset = new Vector2(leftBottom.x - 0.5f - Offset, position.y);
                wallLeft.size = new Vector2(1, (rightTop.y * 2) - (amountOfRise * 2) + (Offset * 2));
                wallRight.offset = new Vector2(rightTop.x + 0.5f + Offset, position.y);
                wallRight.size = new Vector2(1, (rightTop.y * 2) - (amountOfRise * 2) + (Offset * 2));
            }
        }
    }
}