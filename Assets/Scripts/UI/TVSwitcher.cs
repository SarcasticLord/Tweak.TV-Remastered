using UnityEngine;

public class TVSwitcher : MonoBehaviour
{
    public Renderer screen;
    public Texture[] pictures;
    public Texture brokenImage;

    public float changeTime = 5;

    private int currPic = 0;
    private float timer = 0;
    private bool isBroken = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screen.material.mainTexture = pictures[currPic];
    }

    // Update is called once per frame
    void Update()
    {
        if (isBroken)
        {
            return;
        }

        timer += Time.deltaTime;

        if(timer >= changeTime)
        {
            timer = 0f;
            currPic++;

            if(currPic >= pictures.Length)
            {
                currPic = 0;
            }

            screen.material.mainTexture = pictures[currPic];
        }
    }
    public void BreakTV()
    {
        isBroken = true;
        screen.material.mainTexture = brokenImage;
    }
}
