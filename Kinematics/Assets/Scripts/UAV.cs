using UnityEngine;
using UnityEngine.UI;

public class UAV : MonoBehaviour
{
	public Slider theta1;
	public Slider theta2;
	public Slider theta3;
    public Slider dist1;
	public Slider dist2;
	public Slider dist3;

	private Transform uav;

	private float prev_dist1 = 0f;
	private float prev_dist2 = 0f;
	private float prev_dist3 = 0f;

	private float next_dist1 = 0f;
	private float next_dist2 = 0f;
	private float next_dist3 = 0f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		uav = this.transform;

		prev_dist1 = dist1.value;
		prev_dist2 = dist2.value;
		prev_dist3 = dist3.value;

		next_dist1 = dist1.value;
		next_dist2 = dist2.value;
		next_dist3 = dist3.value;

		dist1.onValueChanged.AddListener(delegate { Move(0); });
		dist2.onValueChanged.AddListener(delegate { Move(1); });
		dist3.onValueChanged.AddListener(delegate { Move(2); });
	}

    // Update is called once per frame
    void Update()
    {
		uav.rotation = Quaternion.Euler(theta1.value, theta2.value, theta3.value);
	}

	void Move(int dir)
	{
		int increase_direction = 0;
		float strength = 0f;

		if (dir == 0)
		{
			prev_dist1 = next_dist1;
			next_dist1 = dist1.value;
			if (next_dist1 - prev_dist1 < 0f) increase_direction = -1;
			else if (next_dist1 - prev_dist1 > 0f) increase_direction = 1;
			strength = Mathf.Abs(next_dist1);

			uav.Translate(increase_direction* strength, 0f, 0f);
		}
		else if (dir == 1)
		{
			prev_dist2 = next_dist2;
			next_dist2 = dist2.value;
			if (next_dist2 - prev_dist2 < 0f) increase_direction = -1;
			else if (next_dist2 - prev_dist2 > 0f) increase_direction = 1;
			strength = Mathf.Abs(next_dist2);

			uav.Translate(0f, increase_direction* strength, 0f);
		}
		else if (dir == 2)
		{
			prev_dist3 = next_dist3;
			next_dist3 = dist3.value;
			if (next_dist3 - prev_dist3 < 0f) increase_direction = -1;
			else if (next_dist3 - prev_dist3 > 0f) increase_direction = 1;
			strength = Mathf.Abs(next_dist3);

			uav.Translate(0f, 0f, increase_direction* strength);
		}
	}
}
