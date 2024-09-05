using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Events;

namespace Flamenccio.Localization
{
    public interface IDescribable
    {
        LocalizedString CompleteDescription(LocalizedString description);
    }

    public class ObjectDescription : MonoBehaviour
    {
        [SerializeField] private LocalizedString objectName;
        [SerializeField] private LocalizedString objectDescription;
        [SerializeField] private UnityEventString ues = new();
        private IDescribable dataSource; // Where to source variable values in objectDescription.

        private void Awake()
        {
            FindDataSource();
        }

        private void Start()
        {
            FillDescription();
        }

        public void FillDescription()
        {
            if (dataSource != null)
            {
                objectDescription = dataSource.CompleteDescription(objectDescription);
            }
        }

        public LocalizedString GetObjectName()
        {
            return objectName;
        }

        public LocalizedString GetObjectDescription()
        {
            FillDescription();

            return objectDescription;
        }
        private void FindDataSource()
        {
            if (dataSource != null) return;

            if (!gameObject.TryGetComponent<IDescribable>(out var d))
            {
                Debug.LogError("This game object uses an ObjectDescription component, but does not implement IDescribable!");
            }
            else
            {
                dataSource = d;
            }
        }
    }
}
