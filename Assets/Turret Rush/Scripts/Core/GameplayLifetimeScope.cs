using Turret_Rush.Scripts.Combat;
using Turret_Rush.Scripts.Input;
using Turret_Rush.Scripts.Player;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Turret_Rush.Scripts.Core
{
    public sealed class GameplayLifetimeScope : LifetimeScope
    {
        [Header("Gameplay")]
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private CarController carController;
        [SerializeField] private CarMovement carMovement;
        [SerializeField] private Health carHealth;
        [SerializeField] private Weapon weapon;
        [SerializeField] private GameManager gameManager;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(inputReader);
            builder.RegisterComponent(carController);
            builder.RegisterComponent(carMovement);
            builder.RegisterComponent(carHealth);
            builder.RegisterComponent(weapon);
            builder.RegisterComponent(gameManager);
        }
    }
}