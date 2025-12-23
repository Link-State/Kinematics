using UnityEngine;
using UnityEngine.UI;

public class TwoLink : MonoBehaviour
{
	public Slider slider1;
	public Slider slider2;
    private Transform arm1;
	private Transform arm2;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        arm1 = GameObject.Find("arm1").transform;
        arm2 = GameObject.Find("arm2").transform;
	}

    // Update is called once per frame
    void Update()
    {
        arm1.localRotation = Quaternion.Euler(slider1.value, 0f, -90f);
        arm2.localRotation = Quaternion.Euler(0f, slider2.value, 0f);
	}
}
