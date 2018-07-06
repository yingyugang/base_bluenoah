using UnityEngine;

namespace BlueNoah.UI
{
    public class SampleStartScene : MonoBehaviour
    {

		private void Start()
		{
            UnityEngine.SceneManagement.SceneManager.LoadScene("Sample1");
		}

	}
}
