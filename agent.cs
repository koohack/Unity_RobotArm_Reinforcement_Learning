using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class agent : Agent
{
    private float time_now;
    private float time_max = 2.5f;


    public Joint joint1;
    public Joint joint2;
    public Joint joint3;

    static Quaternion init_joint1;
    static Quaternion init_joint2;
    static Quaternion init_joint3;


    public throw_ball follower;
    public throw_ball followed;

    static float joint1_vel;
    static float joint2_vel;
    static float joint3_vel;


    int checker = 1;
    int restart = 0;

    // Start is called before the first frame update
    void Start()
    {
        //for timer
        time_now = time_max;

        
        //restart the episode
        init_joint1 = joint1.transform.rotation;
        init_joint2 = joint2.transform.rotation;
        init_joint3 = joint3.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position1 = followed.transform.position;

        if (Mathf.Floor(time_now) <= 0)
        {
            int a = 1;
            follower.GetComponent<Rigidbody>().isKinematic = false;
            follower.GetComponent<Rigidbody>().AddForce(3f, 1f, 0f);
        }
        else
        {
            // Rotate continuously
            follower.transform.position = followed.transform.position;

            joint1.GetComponent<Rigidbody>().transform.Rotate(Vector3.up * joint1_vel);
            joint2.GetComponent<Rigidbody>().transform.Rotate(Vector3.up * joint2_vel);
            joint3.GetComponent<Rigidbody>().transform.Rotate(Vector3.up * joint3_vel);
            Debug.Log(" " + joint1_vel.ToString() + " " + joint2_vel.ToString() + " " + joint3_vel.ToString());



            time_now -= Time.deltaTime;
            follower.transform.position = position1;
        }

    }

    
    public Transform Target;
    public override void OnEpisodeBegin()
    {
        //reset the position of the robot arm for every episode
        joint1.transform.rotation = init_joint1;
        joint2.transform.rotation = init_joint2;
        joint3.transform.rotation = init_joint3;

        //reset the position of the target
        //write here

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (checker==1)
        {
            sensor.AddObservation(Target.position);
            sensor.AddObservation(follower.transform.position);

            sensor.AddObservation(joint1_vel);
            sensor.AddObservation(joint2_vel);
            sensor.AddObservation(joint3_vel);
            //sensor.AddObservation(follower.GetComponent<Rigidbody>().velocity.x);
            //sensor.AddObservation(follower.GetComponent<Rigidbody>().velocity.y);
            //add joint 1,2,3 angular velocity 

        }

        

    }

    public float moving = 2;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        joint1_vel = actionBuffers.ContinuousActions[0];
        joint2_vel = actionBuffers.ContinuousActions[1];
        joint3_vel = actionBuffers.ContinuousActions[2];


        Debug.Log("here i am");
        joint1.GetComponent<Rigidbody>().transform.Rotate(Vector3.up * joint1_vel);
        joint2.GetComponent<Rigidbody>().transform.Rotate(Vector3.up * joint2_vel);
        joint3.GetComponent<Rigidbody>().transform.Rotate(Vector3.up * joint3_vel);

        float distance = Vector3.Distance(Target.localPosition, follower.transform.localPosition);
        if(follower.transform.position.y < 0.5 && distance < 0.7)
        {
            SetReward(1.0f);
            EndEpisode();
        }else if(follower.transform.position.x > 100){
            EndEpisode();
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
        continuousActionsOut[2] = Input.GetAxis("Horizontal");
    }


}
