using UnityEngine;

public class LocalMinimum : MonoBehaviour
{
    public Transform goal;
    public Transform obstacles;
    public float epsilon = 1f;
    public float zeta = 0.03f;
	public float eta = 1f;
	public float delta = 0.03f;

    private Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = this.transform;

        epsilon = 2f;
		zeta = 0.03f;
		eta = 2f;
		delta = 2f;
	}

    // Update is called once per frame
    void Update() { }

	private void FixedUpdate()
	{
        player.position = player.position + (F_att() + F_rep());
	}

    private Vector3 F_att() {
        Vector3 dist_vec = player.position - goal.position;
        float dist = dist_vec.magnitude;
        if (dist > epsilon) {
            return - (( epsilon * zeta * dist_vec ) / dist);
        }

        return -(zeta * dist_vec);
    }

    private Vector3 F_rep() {
        Transform obstacle;
        Vector3 dist_vec;
        float dist;
        Vector3 total_vec = new Vector3(0f, 0f, 0f);

		for (int i = 0; i < obstacles.childCount; i++) {
            obstacle = obstacles.GetChild(i);

            dist_vec = player.position - obstacle.position;
            dist = dist_vec.magnitude;
			if (dist < delta) {
                total_vec += eta * ((1 / dist) - (1 / delta)) * (1 / (dist * dist)) * (dist_vec / dist);
            }
        }

        return total_vec;
    }
}
