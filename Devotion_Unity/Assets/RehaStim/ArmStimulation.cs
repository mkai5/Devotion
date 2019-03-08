using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/**
 * Interface Class for easier access to RehaStim
 * This script should only exist once in the scene.
 */

public class ArmStimulation : MonoBehaviour
{
    public static ArmStimulation instance = null;
	
	/* DIRECTION OF MOVEMENT (VERTICAL)
	0 - Down
	1 - Up
	2 - Stationary	*/
	[HideInInspector] public int v_direction = 0;
	[HideInInspector] public List<float> y_positions = new List<float>();
	[HideInInspector] public bool list_full;


    public int frequency = 75;

    public int pulseCount = 5;

    [Header("Flexor")]
    public Channels channelWristL = Channels.Channel1;
	public int testWidthWL = 200;
	public int testCurrentWL = 9;
	
    [HideInInspector]public Channels channelBicepsL = Channels.Channel2;
	[HideInInspector]public int testWidthBL=200;
	[HideInInspector]public int testCurrentBL=9;
    [HideInInspector]public Channels channelTricepsL = Channels.Channel3;
	[HideInInspector]public int testWidthTL=200;
	[HideInInspector]public int testCurrentTL=9;
    [HideInInspector]public Channels channelShoulderL = Channels.Channel4;
	[HideInInspector]public int testWidthSL=200;
	[HideInInspector]public int testCurrentSL=9;
    [HideInInspector]public Channels channelInwardsWristL = Channels.ChannelNone;
	[HideInInspector]public int testWidthIWL=200;
	[HideInInspector]public int testCurrentIWL=9;
    [HideInInspector]public Channels channelOutwardsWristL = Channels.ChannelNone;
	[HideInInspector]public int testWidthOWL=200;
	[HideInInspector]public int testCurrentOWL=9;

    [Header("Extensor")]
    public Channels channelWristR = Channels.Channel2;
	public int testWidthWR=200;
	public int testCurrentWR=9;
	
    [HideInInspector]public Channels channelBicepsR = Channels.Channel6;
	[HideInInspector]public int testWidthBR=200;
	[HideInInspector]public int testCurrentBR=9;
    [HideInInspector]public Channels channelTricepsR = Channels.Channel7;
	[HideInInspector]public int testWidthTR=200;
	[HideInInspector]public int testCurrentTR=9;
    [HideInInspector]public Channels channelShoulderR = Channels.Channel8;
	[HideInInspector]public int testWidthSR=200;
	[HideInInspector]public int testCurrentSR=9;
    [HideInInspector]public Channels channelInwardsWristR = Channels.ChannelNone;
	[HideInInspector]public int testWidthIWR=200;
	[HideInInspector]public int testCurrentIWR=9;
    [HideInInspector]public Channels channelOutwardsWristR = Channels.ChannelNone;
	[HideInInspector]public int testWidthOWR=200;
	[HideInInspector]public int testCurrentOWR=9;

    [Header("Testing")]
    public Part testPart = Part.Wrist;

	public int lWidth = 200;
	public int lCurrent = 9;

	public int rWidth = 200;
	public int rCurrent = 9;
	
	[Header("Effects")]
	public bool vibrato = false;
	
	[Header("Stage")]
	public int stage = 0;

    /*
     * Static call to stimulate
     */

    public static void StimulateArm(Part part, Side side, int width, int current)
    {
        if (instance)
            instance.stimulate(part, side, width, current);
    }

    public static void StimulateArmSinglePulse(Part part, Side side, int width, int current)
    {
        if (instance)
            instance.stimulateSinglePulse(part, side, width, current);
    }

    public static void StimulateArmBurst(StimulationInfo info, int duration)
    {
        if (instance)
            instance.StartCoroutine(instance.stimulateBurst(new StimulationInfo[] { info }, duration));
    }

    public static void StimulateArmBurst(StimulationInfo[] infos, int duration)
    {
        if (instance)
            instance.StartCoroutine(instance.stimulateBurst(infos, duration));
    }
	
	public static void StimulateVibrato()
    {
        if (instance)
            instance.StartCoroutine(instance.stimulateVibrato());
    }
	
    private void Update()
    {
		double y_average = y_positions.Count > 0 ? y_positions.Average() : 0.0;

		if ((y_average-this.transform.position.y)>0.01){
			v_direction=0;
		} else if ((y_average-this.transform.position.y)<-0.01){
			v_direction=1;
		} 
		//else {
		//	v_direction=2; //prevents oscillation
		//}
		
		lWidth=GetTestWidth(testPart, Side.Left);
		lCurrent=GetTestCurrent(testPart, Side.Left);
		rWidth=GetTestWidth(testPart, Side.Right);
		rCurrent=GetTestCurrent(testPart, Side.Right);
		
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
			stage=0;
			
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
			stage++;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
			Debug.Log("Stimulation stopping (from ArmStimulation.cs)");
            Stop();
			
        }
		else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
			StimulateVibrato();
		}

        y_positions.Add(this.transform.position.y);
		if (list_full){
			y_positions.RemoveAt(0);
		} else if (y_positions.Count > 5)
			list_full=true;
	}
			

    // Use this for initialization
    private void Start()
    {
        instance = this;
		list_full = false;

    }

    private void Stop()
    {
        if (ChannelList.isRunning)
        {
            ChannelList.Stop();
        }
    }

    public void initStimulation()
    {
        if (!ChannelList.isRunning)
        {
            //init EMS
            ChannelMask channels = new ChannelMask();
            channels.setChannel(channelWristL);
            channels.setChannel(channelBicepsL);
            channels.setChannel(channelTricepsL);
            channels.setChannel(channelShoulderL);

            channels.setChannel(channelWristR);
            channels.setChannel(channelBicepsR);
            channels.setChannel(channelTricepsR);
            channels.setChannel(channelShoulderR);

            channels.setChannel(channelInwardsWristL);
            channels.setChannel(channelOutwardsWristL);

            channels.setChannel(channelInwardsWristR);
            channels.setChannel(channelOutwardsWristR);

            ChannelList.InitCM(0
                    , channels
                    , new ChannelMask() //low freq channels not used
                    , frequency
                    , frequency);
        }
    }

    public void stimulate(Part part, Side side, int width, int current)
    {
        //init channel mode if needed
        initStimulation();
		Debug.Log("Stimulation initiated");

        //update bodypart
        Channels channel = GetChannel(part, side);

        if (channel == Channels.ChannelNone)
            return;

        UpdateInfo info = new UpdateInfo();
        info.pulseCurrent = Mathf.Max(0, current);
        info.pulseWidth = Mathf.Max(0, width);

        //update EMS
        ChannelList.UpdateChannel(channel, info);
		Debug.Log("Stimulation ended");
    }

    protected Channels GetChannel(Part part, Side side)
    {
        Channels channel;
        switch (part)
        {
            case (Part.Wrist):
                channel = side == Side.Left ? channelWristL : channelWristR;
                break;

            case (Part.Biceps):
                channel = side == Side.Left ? channelBicepsL : channelBicepsR;
                break;

            case (Part.Triceps):
                channel = side == Side.Left ? channelTricepsL : channelTricepsR;
                break;

            case (Part.Shoulder):
                channel = side == Side.Left ? channelShoulderL : channelShoulderR;
                break;

            case (Part.InwardsWrist):
                channel = side == Side.Left ? channelInwardsWristL : channelInwardsWristR;
                break;

            case (Part.OutwardsWrist):
                channel = side == Side.Left ? channelOutwardsWristL : channelOutwardsWristR;
                break;

            default:
                return Channels.ChannelNone;
        }

        return channel;
    }
	
	private int GetTestWidth(Part part, Side side)
    {
		int w;
        switch (part)
        {
            case (Part.Wrist):
                w = side == Side.Left ? testWidthWL : testWidthWR;
                break;

            case (Part.Biceps):
                w = side == Side.Left ? testWidthBL : testWidthBR;
                break;

            case (Part.Triceps):
                w = side == Side.Left ? testWidthTL : testWidthTR;
                break;

            case (Part.Shoulder):
                w = side == Side.Left ? testWidthSL : testWidthSR;
                break;

            case (Part.InwardsWrist):
                w = side == Side.Left ? testWidthIWL : testWidthOWR;
                break;

            case (Part.OutwardsWrist):
                w = side == Side.Left ? testWidthOWL : testWidthOWR;
                break;

            default:
                return 0;
        }

        return w;
    }
	
	private int GetTestCurrent(Part part, Side side)
    {
		int c;
        switch (part)
        {
            case (Part.Wrist):
                c = side == Side.Left ? testCurrentWL : testCurrentWR;
                break;

            case (Part.Biceps):
                c = side == Side.Left ? testCurrentBL : testCurrentBR;
                break;

            case (Part.Triceps):
                c = side == Side.Left ? testCurrentTL : testCurrentTR;
                break;

            case (Part.Shoulder):
                c = side == Side.Left ? testCurrentSL : testCurrentSR;
                break;

            case (Part.InwardsWrist):
                c = side == Side.Left ? testCurrentIWL : testCurrentOWR;
                break;

            case (Part.OutwardsWrist):
                c = side == Side.Left ? testCurrentOWL : testCurrentOWR;
                break;

            default:
                return 0;
        }

        return c;
    }


    public void stimulateSinglePulse(Part part, Side side, int width, int current)
    {
        Stop();

        //update bodypart
        Channels channel = GetChannel(part, side);

        if (channel == Channels.ChannelNone)
            return;

        for (int i = 0; i < pulseCount; i++)
        {
            SinglePulse.sendSinglePulse(channel, width, current);
		}
    }
	
	/*public void stimulateVibrato()
    {
        Stop();

		Debug.Log("vibrato subroutine");
		
        for (int i = 0; i < 20; i++)
        {
            SinglePulse.sendSinglePulse(1, 200, 9);
			yield return new WaitForSeconds(0.1);
		}
		yield return new WaitForSeconds(0.3);
		for (int i = 0; i < 20; i++)
        {
            SinglePulse.sendSinglePulse(2, 200, 12);
			yield return new WaitForSeconds(0.1);
		}	
		
    }*/
	
	public IEnumerator stimulateVibrato()
    {
        Stop();
           while(stage==1){
            for (int i = 0; i < 20; i++)
			{
				SinglePulse.sendSinglePulse(1, 200, 7);
				yield return new WaitForSeconds(0.003f);
			}
			
			yield return new WaitForSeconds(0.08f);
			
			for (int i = 0; i < 20; i++)
			{
				SinglePulse.sendSinglePulse(2, 200, 9);
				yield return new WaitForSeconds(0.003f);
			}	
				
			yield return new WaitForSeconds(0.08f);
			
			yield return new WaitForSeconds(0.08f);
		   }
		   
		   while(stage==2){
            for (int i = 0; i < 20; i++)
			{
				SinglePulse.sendSinglePulse(1, 200, 8);
				yield return new WaitForSeconds(0.003f);
			}
			
			yield return new WaitForSeconds(0.07f);
			
			for (int i = 0; i < 20; i++)
			{
				SinglePulse.sendSinglePulse(2, 200, 11);
				yield return new WaitForSeconds(0.003f);
			}	
				
			yield return new WaitForSeconds(0.07f);
			
			yield return new WaitForSeconds(0.07f);
		   }
		   
		   while(stage==3){
            for (int i = 0; i < 20; i++)
			{
				SinglePulse.sendSinglePulse(1, 200, 9);
				yield return new WaitForSeconds(0.0025f);
			}
			
			yield return new WaitForSeconds(0.06f);
			
			for (int i = 0; i < 20; i++)
			{
				SinglePulse.sendSinglePulse(2, 200, 12);
				yield return new WaitForSeconds(0.0025f);
			}	
				
			yield return new WaitForSeconds(0.06f);
			
			yield return new WaitForSeconds(0.06f);
		   }
		   
		   
		   while(stage==4){
            for (int i = 0; i < 15; i++)
			{
				SinglePulse.sendSinglePulse(1, 200, 10);
				yield return new WaitForSeconds(0.001f);
			}
			
			yield return new WaitForSeconds(0.04f);
			
			for (int i = 0; i < 15; i++)
			{
				SinglePulse.sendSinglePulse(2, 200, 12);
				yield return new WaitForSeconds(0.001f);
			}	
				
			yield return new WaitForSeconds(0.04f);
			
			yield return new WaitForSeconds(0.04f);
		   }
		   
		   while(stage==5){
            for (int i = 0; i < 12; i++)
			{
				SinglePulse.sendSinglePulse(1, 200, 10);
				yield return new WaitForSeconds(0.001f);
			}
			
			yield return new WaitForSeconds(0.03f);
			
			for (int i = 0; i < 12; i++)
			{
				SinglePulse.sendSinglePulse(2, 200, 13);
				yield return new WaitForSeconds(0.001f);
			}	
				
			yield return new WaitForSeconds(0.03f);
			
			yield return new WaitForSeconds(0.03f);
		   }
		   
		   while(stage==6){
            for (int i = 0; i < 8; i++)
			{
				SinglePulse.sendSinglePulse(1, 200, 11);
				yield return new WaitForSeconds(0.001f);
			}
			
			yield return new WaitForSeconds(0.03f);
			
			for (int i = 0; i < 8; i++)
			{
				SinglePulse.sendSinglePulse(2, 200, 14);
				yield return new WaitForSeconds(0.001f);
			}	
				
			yield return new WaitForSeconds(0.03f);
			
			yield return new WaitForSeconds(0.03f);
		   }
		   
		   while(stage==7){
            for (int i = 0; i < 8; i++)
			{
				SinglePulse.sendSinglePulse(1, 200, 11);
				yield return new WaitForSeconds(0.001f);
			}
			
			yield return new WaitForSeconds(0.03f);
			
			for (int i = 0; i < 8; i++)
			{
				SinglePulse.sendSinglePulse(2, 200, 15);
				yield return new WaitForSeconds(0.001f);
			}	
				
			yield return new WaitForSeconds(0.025f);
			
			yield return new WaitForSeconds(0.025f);
		   }
		   
		   while(stage==8){
            for (int i = 0; i < 5; i++)
			{
				SinglePulse.sendSinglePulse(1, 200, 12);
				yield return new WaitForSeconds(0.001f);
			}
			
			yield return new WaitForSeconds(0.03f);
			
			for (int i = 0; i < 5; i++)
			{
				SinglePulse.sendSinglePulse(2, 200, 15);
				yield return new WaitForSeconds(0.001f);
			}	
				
			yield return new WaitForSeconds(0.02f);
			
			yield return new WaitForSeconds(0.02f);
		   }
		   
		   while(stage==9){
            for (int i = 0; i < 5; i++)
			{
				SinglePulse.sendSinglePulse(1, 200, 12);
				yield return new WaitForSeconds(0.001f);
			}
			
			yield return new WaitForSeconds(0.01f);
			
			for (int i = 0; i < 5; i++)
			{
				SinglePulse.sendSinglePulse(2, 250, 15);
				yield return new WaitForSeconds(0.001f);
			}	
				
			yield return new WaitForSeconds(0.01f);
			
			yield return new WaitForSeconds(0.01f);
		   }
		   
		   while(stage==10){
            for (int i = 0; i < 4; i++)
			{
				SinglePulse.sendSinglePulse(1, 225, 12);
				yield return new WaitForSeconds(0.001f);
			}
			
			//yield return new WaitForSeconds(0.01f);
			
			for (int i = 0; i < 8; i++)
			{
				SinglePulse.sendSinglePulse(2, 250, 15);
				yield return new WaitForSeconds(0.001f);
			}	
				
			yield return new WaitForSeconds(0.01f);
			
			//yield return new WaitForSeconds(0.01f);
		   }


            //yield return null;
    
    }

    public IEnumerator stimulateBurst(StimulationInfo[] infos, int duration)
    {
        System.DateTime stopTime = System.DateTime.Now.AddMilliseconds(duration);

        while (stopTime > System.DateTime.Now)
        {
            Stop();
            foreach (var info in infos)
            {
                Channels channel = GetChannel(info.part, info.side);
                SinglePulse.sendSinglePulse(channel, info.width, info.current);
            }

            yield return null;
        }
    }

    public void OnTriggerEnter (Collider collider)
    {

        Debug.Log("Entered collider");
        if (collider.gameObject.tag == "vertical fret"){
            if (v_direction==0){
				stimulateSinglePulse(testPart, Side.Right, GetTestWidth(testPart, Side.Right),
				GetTestCurrent(testPart, Side.Right));
			} else if (v_direction==1){
				stimulateSinglePulse(testPart, Side.Left, GetTestWidth(testPart, Side.Left),
				GetTestCurrent(testPart, Side.Left));
			}
		} else if (collider.gameObject.tag == "vibrato toggle"){
			Debug.Log("entered box");
			if (vibrato){
				vibrato=false;
			}
			else {
				vibrato=true;
				stimulateVibrato();
			}
		}
		/* 	TO FILL IN AFTER FINISHING VERTICAL FRETS
		else if (collider.gameObject.tag == "Horizontal Fret"){
            // Collider behavior horizontal fret
        }*/		
    }
	
	public void OnTriggerStay (Collider collider)
	{
		if (collider.gameObject.tag == "lower bound"){
				stimulate(testPart, Side.Right, GetTestWidth(testPart, Side.Right),
				GetTestCurrent(testPart, Side.Right));
			} else if (collider.gameObject.tag == "upper bound"){
				stimulate(testPart, Side.Left, GetTestWidth(testPart, Side.Left),
				GetTestCurrent(testPart, Side.Left));
			}
	}

    public void OnTriggerExit (Collider collider){
        if (collider.gameObject.tag== "bound")
            Stop();
    }
}

public class StimulationInfo
{
    public Part part;
    public Side side;
    public int width;
    public int current;

    public StimulationInfo(Part part, Side side, int width, int current)
    {
        this.part = part;
        this.side = side;
        this.width = width;
        this.current = current;
    }
}

public enum Part
{
    Wrist = 1,
    Biceps,
    Triceps,
    Shoulder,
    OutwardsWrist,
    InwardsWrist,
}

public enum Side
{
    Left = 1,
    Right = 2,
}