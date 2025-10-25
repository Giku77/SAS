using UnityEngine;
using UnityEngine.UI;

namespace CrusaderUI.Scripts
{
	public class HPFlowController : MonoBehaviour {
	
		private Material _material;
		private float max;
        private void Start ()
		{
			_material = GetComponent<Image>().material;
		}

		public void SetValue(float value, float max)
		{
            this.max = max;

            _material.SetFloat("_FillLevel", value / max);
		}
        public void UpdateMpValue(float value)
        {
            _material.SetFloat("_FillLevel", value / max);
        }

        public void SetValue(float value)
        {
            _material.SetFloat("_FillLevel", value / 100);
        }
    }
}