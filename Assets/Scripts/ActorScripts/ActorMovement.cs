﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorMovement : MonoBehaviour, ActorActionReceiver {

    private Rigidbody rigidBody;

    private Vector3 MostRecentVectorDeclaration;
    private float timeDelay = 0;

    private float momentumDuration = 0.5f;
    private float momentumTime;
    private Vector3 Momentum;

    private float VerticalVelocity = 0;
    private float Gravity = Physics.gravity.y;

    private bool InAir = false;
    private int JumpsAvailable;

    [SerializeField]
    private ActorMechanics Mechanics;

    public void InterpretAction(ActorAction action)
    {
        if(action is Move && timeDelay <= 0 && !InAir)
        {
            DeclareMovement((action as Move).MovementVector.normalized);
            action.Interpret();       
        }

        if (action is DodgeRoll && timeDelay <= 0 && !InAir)
        {
            DeclareMovement((action as DodgeRoll).MovementVector.normalized);
            action.Interpret();
        }

        if (action is Jump && JumpsAvailable > 0)
        {
            VerticalVelocity = Mechanics.Movement.JumpSpeed;
            InAir = true;
            JumpsAvailable--;
            action.Interpret();
        }

        if (action.Interpreted)
        {
            timeDelay += action.BaseTimeDelay;
        }
    }

    public void DeclareMovement(Vector3 mov)
    {
        MostRecentVectorDeclaration = mov;
    }

    // Use this for initialization
    void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();
        if (rigidBody == null) Debug.Log("Nie znaleziono RigidBody");

        MostRecentVectorDeclaration = Vector3.zero;
        Momentum = Vector3.zero;

        rigidBody.velocity = Vector3.zero;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        //float speed = Mechanics.GetFloatStatValue(ActorStatsDeclaration.Speed);
        float speed = Mechanics.Movement.Speed;
        float delta = 1.0f / 60;

        if (MostRecentVectorDeclaration != Vector3.zero)
        {
            Momentum = MostRecentVectorDeclaration;
            momentumTime = momentumDuration;
        }
        else
        {
            if(!InAir)
                momentumTime = Mathf.Clamp(momentumTime - delta, 0, momentumDuration);            
        }

        rigidBody.velocity = (Momentum * momentumTime / momentumDuration) * speed + new Vector3(0, VerticalVelocity, 0);

        if (InAir) VerticalVelocity += Gravity * delta;
        else VerticalVelocity = 0;

        MostRecentVectorDeclaration = Vector3.zero;        

        if (timeDelay > 0) timeDelay -= delta;
    }  
    
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("CollisionEnter");

        InAir = false;
        //JumpsAvailable = (int)Mechanics.GetFloatStatValue(ActorStatsDeclaration.JumpsAllowed);
        JumpsAvailable = (int)Mechanics.Movement.JumpsAvailable;
        VerticalVelocity = 0;
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("CollisionExit");
        InAir = true;
    }
}
