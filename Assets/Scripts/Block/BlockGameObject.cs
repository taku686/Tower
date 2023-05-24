using System;
using Cysharp.Threading.Tasks;
using Data;
using UniRx;
using UnityEngine;

namespace Block
{
    public class BlockGameObject : MonoBehaviour
    {
        public readonly ReactiveProperty<BlockSate> BlockStateReactiveProperty = new() { Value = BlockSate.Generating };
        private Rigidbody2D _rigidbody2D;
        public int index;
        public bool isOwn;
        private const float StopThreshold = 0.01f;

        private void OnEnable()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public void Initialize(BlockSate state, int blockIndex)
        {
            BlockStateReactiveProperty.Value = state;
            index = blockIndex;
            isOwn = true;
        }

        private void FixedUpdate()
        {
            if (_rigidbody2D == null)
            {
                return;
            }

            if (BlockStateReactiveProperty.Value == BlockSate.Stop)
            {
                return;
            }

            if (_rigidbody2D.velocity.y < 0)
            {
                BlockStateReactiveProperty.Value = BlockSate.Moving;
                return;
            }


            if ((BlockStateReactiveProperty.Value == BlockSate.ReMove ||
                 BlockStateReactiveProperty.Value == BlockSate.Moving) &&
                _rigidbody2D.velocity.magnitude <= StopThreshold)
            {
                _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
                BlockStateReactiveProperty.Value = BlockSate.Stop;
                Destroy(_rigidbody2D);
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (_rigidbody2D == null)
            {
                return;
            }

            if (col.collider.CompareTag(GameCommonData.GroundTag) || col.collider.CompareTag(GameCommonData.BlockTag))
            {
                _rigidbody2D.gravityScale = GameCommonData.GravityScale;
            }
        }

        private void OnDestroy()
        {
            BlockStateReactiveProperty.Dispose();
        }
    }

    public enum BlockSate
    {
        Generating,
        Operating,
        Rotating,
        Moving,
        Stop,
        ReMove
    }
}