using TMPro;
using UnityEngine;

namespace Source.Code
{
    public class ProductVisualization : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMeshPro;

        public void UpdateValue(int amount)
        {
            textMeshPro.SetText(amount.ToString());
        }
    }
}
