using System.Collections.Generic;
using UnityEngine;
using RuntimeFSM.Interfaces;

namespace RuntimeFSM.Examples
{
    public class ExampleActionExecutor : MonoBehaviour, IActionExecutor
    {
        private Animator _animator;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Execute(string actionName, Dictionary<string, string> parameters)
        {
            switch (actionName)
            {
                case "PlayAnim":
                    ExecutePlayAnim(parameters);
                    break;

                case "Move":
                    ExecuteMove(parameters);
                    break;
            }
        }

        private void ExecutePlayAnim(Dictionary<string, string> parameters)
        {
            if (_animator == null) return;

            if (parameters != null && parameters.TryGetValue("Name", out var animName))
            {
                _animator.Play(animName);
            }
        }

        private void ExecuteMove(Dictionary<string, string> parameters)
        {
            if (_rb == null) return;

            if (parameters != null &&
                parameters.TryGetValue("Speed", out var speedStr) &&
                float.TryParse(speedStr, out var speed))
            {
                // Movimiento horizontal simple
                _rb.linearVelocity = new Vector2(speed, _rb.linearVelocity.y);
            }
        }
    }
}
