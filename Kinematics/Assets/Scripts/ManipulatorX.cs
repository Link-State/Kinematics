using UnityEngine;
using UnityEngine.UI;

public class ManipulatorX : MonoBehaviour
{
	public Slider slider1;
	public Slider slider2;
    public Slider slider3;
	private Transform arm2;
	private Transform arm3;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		arm2 = GameObject.Find("arm2").transform;
		arm3 = GameObject.Find("arm3").transform;
	}

    // Update is called once per frame
    void Update()
    {
		this.transform.localRotation = Quaternion.Euler(0f, slider1.value, 0f);
		arm2.localRotation = Quaternion.Euler(0f, 0f, slider2.value);
		arm3.localRotation = Quaternion.Euler(0f, 0f, slider3.value);
	}
}
