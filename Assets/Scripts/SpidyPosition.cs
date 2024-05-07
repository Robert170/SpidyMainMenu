using UnityEngine;

public class SpidyPosition : MonoBehaviour
{
    public Spidy spidy;

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            StaticVar.doors.Add(Vector3.zero);
        }
        switch (StaticVar.CurrentLevel)
        {
            case 0:
                spidy.transform.position = new Vector3(3088, 729.0084f, 4320);
                if (!GameObject.Find("MyGameManager").
                    GetComponent<GameManager>().hasRemainingEvents && StaticVar.signal1)
                {
                    spidy.transform.position = new Vector3(1036, 729.0084f, 4238f);
                }
                StaticVar.doors[0] = new Vector3(0.0f, 0.0f, 0.0f);
                StaticVar.doors[1] = new Vector3(1036.0f, 729.0084f, 4238f);
                StaticVar.doors[2] = new Vector3(0.0f, 0.0f, 0.0f);
                StaticVar.doors[3] = new Vector3(0.0f, 0.0f, 0.0f);
                break;
            case 1:
                spidy.transform.position = new Vector3(1275, 729.0084f, 5115);
                if (StaticVar.signal2)
                {
                    spidy.transform.position = new Vector3(1275, 729.0084f, 5115);
                }
                else if (StaticVar.signal1)
                {
                    spidy.transform.position = new Vector3(3244, 729.0084f, 4652);
                }
                StaticVar.doors[0] = new Vector3(1275.0f, 729.0084f, 5115.0f);
                StaticVar.doors[1] = new Vector3(3244.0f, 729.0084f, 4652.0f);
                StaticVar.doors[2] = new Vector3(0.0f, 0.0f, 0.0f);
                StaticVar.doors[3] = new Vector3(0.0f, 0.0f, 0.0f);
                break;
            case 2:
                spidy.transform.position = new Vector3(611.7f, 729.0084f, 4598f);
                if (StaticVar.signal2)
                {
                    spidy.transform.position = new Vector3(611.7f, 729.0084f, 4598f);
                }
                else if (StaticVar.signal1)
                {
                    spidy.transform.position = new Vector3(4664.854f, 729.0084f, 5118.954f);
                }
                StaticVar.doors[0] = new Vector3(611.7f, 729.0084f, 4598f);
                StaticVar.doors[1] = new Vector3(2900f, 729.0084f, 5120.0f);
                StaticVar.doors[2] = new Vector3(3650.0f, 729.0084f, 4500.0f);
                StaticVar.doors[3] = new Vector3(4664.854f, 729.0084f, 5118.954f);
                break;
            case 3:
                spidy.transform.position = new Vector3(277.0f, 729.0084f, 4545.0f);
                if (StaticVar.signal2)
                {
                    spidy.transform.position = new Vector3(277.0f, 729.0084f, 4545.0f);
                }
                else if (StaticVar.signal1)
                {
                    spidy.transform.position = new Vector3(6495.0f, 1657.008f, 5930.0f);
                }
                StaticVar.doors[0] = new Vector3(277.0f, 729.0084f, 4545.0f);
                StaticVar.doors[1] = new Vector3(6495.0f, 1657.008f, 5930.0f);
                StaticVar.doors[2] = new Vector3(850.0f, 1657.008f, 5330.0f);
                StaticVar.doors[3] = new Vector3(6540.0f, 729.0084f, 4765.0f);
                break;
            case 4:
                spidy.transform.position = new Vector3(2160.0f, 1703.017f, 5730.0f);
                if (StaticVar.signal2)
                {
                    spidy.transform.position = new Vector3(2160.0f, 1703.017f, 5730.0f);
                }
                else if (StaticVar.signal1)
                {
                    spidy.transform.position = new Vector3(6500.0f, 767.0167f, 4430f);
                }
                StaticVar.doors[0] = new Vector3(2160.0f, 1703.017f, 5730.0f);
                StaticVar.doors[1] = new Vector3(6500.0f, 767.0167f, 4430f);
                StaticVar.doors[2] = new Vector3(3600f, 767.0167f, 3950f);
                StaticVar.doors[3] = new Vector3(220f, 767.0167f, 4470f);
                break;
            case 5:
                spidy.transform.position = new Vector3(630.0f, 692.8203f, 4150f);
                if (StaticVar.signal2)
                {
                    spidy.transform.position = new Vector3(630.0f, 692.8203f, 4150f);
                }
                else if (StaticVar.signal1)
                {
                    spidy.transform.position = new Vector3(2905f, 692.8203f, 4467.348f);
                }
                StaticVar.doors[0] = new Vector3(565.0f, 692.8203f, 4330f);
                StaticVar.doors[1] = new Vector3(2910.0f, 692.8203f, 4500f);
                StaticVar.doors[2] = new Vector3(3600f, 692.8203f, 4260f);
                StaticVar.doors[3] = new Vector3(0.0f, 0.0f, 0.0f);
                break;
            case 6:
                spidy.transform.position = new Vector3(553f, 1263.008f, 4224f);
                if (StaticVar.signal2)
                {
                    spidy.transform.position = new Vector3(553f, 1263.008f, 4224f);
                }
                else if (StaticVar.signal1)
                {
                    spidy.transform.position = new Vector3(2913f, 1263.008f, 4150f);
                }
                StaticVar.doors[0] = new Vector3(553.0f, 1263.008f, 4224.0f);
                StaticVar.doors[1] = new Vector3(2913.0f, 1263.008f, 4150.0f);
                StaticVar.doors[2] = new Vector3(0.0f, 0.0f, 0.0f);
                StaticVar.doors[3] = new Vector3(0.0f, 0.0f, 0.0f);
                break;
        }
        StaticVar.signal1 = false;
        StaticVar.signal2 = false;
        StaticVar.signal3 = false;
    }
}
