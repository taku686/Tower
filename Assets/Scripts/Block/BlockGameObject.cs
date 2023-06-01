using Data;
using Photon.Pun;
using UniRx;
using UnityEngine;

namespace Block
{
    public class BlockGameObject : MonoBehaviour
    {
        public readonly ReactiveProperty<BlockSate> BlockStateReactiveProperty = new() { Value = BlockSate.Generating };
        private Rigidbody2D _rigidbody2D;
        private PhotonView _photonView;
        public int index;
        public bool isOwn;
        [SerializeField] private float stopThreshold = 0.001f;

        private void OnEnable()
        {
            _photonView = GetComponent<PhotonView>();
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
                _rigidbody2D.velocity.magnitude <= stopThreshold)
            {
                BlockStateReactiveProperty.Value = BlockSate.Stop;
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