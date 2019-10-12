using System;
using System.Collections;
using UnityEngine;

namespace Entities
{
    public class Entity : MonoBehaviour
    {
        public RoomConfiguration.TileType type;
        private Animator animator;

        private void Start()
        {
            animator = this.GetComponent<Animator>();
        }

        public void MoveTo(Vector3 position, Vector2Int direction)
        {
            if (direction == Vector2Int.left || direction == Vector2Int.up)
                animator.SetTrigger("SX");
            else if (direction == Vector2Int.right || direction == Vector2Int.down)
                animator.SetTrigger("DX");

            StartCoroutine(Movement(position));
        }

        private IEnumerator Movement(Vector3 position)
        {
            float distance = (position - transform.position).magnitude;

            while (distance > 0.1f)
            {
                distance = (position - transform.position).magnitude;

                this.transform.position = Vector3.Lerp(transform.position, position, Mathf.Max(distance / 10, 0.1f));
                yield return null;
            }

            this.transform.position = position;
        }
    }
}