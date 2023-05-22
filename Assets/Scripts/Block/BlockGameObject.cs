using System;
using Data;
using UniRx;
using UnityEngine;

namespace Block
{
    [RequireComponent(typeof(Rigidbody2D))]
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
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.collider.CompareTag(GameCommonData.GroundTag) || col.collider.CompareTag(GameCommonData.BlockTag))
            {
                _rigidbody2D.gravityScale = 1;
            }
        }

        private void OnBecameInvisible()
        {
            if (BlockStateReactiveProperty.Value != BlockSate.Stop)
            {
                return;
            }

            _rigidbody2D.Sleep();
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