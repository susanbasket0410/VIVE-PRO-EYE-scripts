using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;

namespace ViveSR.anipal.Eye
{

  ///<summary>
  /// This source code is based on SRanipal_Unity_SDK version 1.1.0.1.
  /// This class collects eye movement data measured by VIVE-PRO-EYE.
  ///</summary>

  public class GetEyeDataModule : MonoBehaviour
  {
    private static EyeData_v2 eyeData = new EyeData_v2();
    private bool eye_callback_registered = false;

    public static int timeStamp;
    public static Vector3 gazeOriginLeft, gazeOriginRight;
    public static Vector3 gazeDirectionRight, gazeDirectionLeft, gazeDirectionCombined;
    public static float pupilDiameterLeft, pupilDiameterRight, pupilDiameterCombined;
    public static float eyeOpenLeft, eyeOpenRight, eyeOpenCombined;
    public static Vector2 pupilPositionLeft, pupilPositionRight, pupilPositionCombined;
    

    readonly static object DebugWriter = new object();
    private CSVWriter csvwriter;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
      // Whether to enable anipal's Eye module
      if (!SRanipal_Eye_Framework.Instance.EnableEye) return;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
      Debug.Log("update fugafuga");

      // check the status of the anipal engine before getting eye data
      if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
          SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

      // the spellｓ to use a callback function to get the measurement data at 120fps
      if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
      {
        SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
        eye_callback_registered = true;
      }
      else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
      {
        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
        eye_callback_registered = false;
      }
      
    }

    /// <summary>
    /// It's called at 120 fps to get more accurate data
    /// </summary>
    private static void EyeCallback(ref EyeData_v2 eye_data)
    {
      Debug.Log("callback hogehoge");
      // Gets data from anipal's Eye module
      eyeData = eye_data;

      // The time when the frame was capturing. in millisecond.
      timeStamp = eyeData.timestamp;

      // The point in the eye from which the gaze ray originates in meter miles.(right-handed coordinate system)
      gazeOriginLeft = eyeData.verbose_data.left.gaze_origin_mm;
      gazeOriginRight = eyeData.verbose_data.Right.gaze_origin_mm;
      Debug.Log("gazeOriginLeft: " + gazeOriginLeft);

      // The normalized gaze direction of the eye in [0,1].(right-handed coordinate system)
      gazeDirectionLeft = eyeData.verbose_data.left.gaze_direction_normalized;
      gazeDirectionRight = eyeData.verbose_data.right.gaze_direction_normalized;
      gazeDirectionCombined = eyeData.verbose_data.combined.eye_data.gaze_direction_normalized; 
      Debug.Log("gaze_direction_left: " + gazeDirectionLeft);

      // The diameter of the pupil in milli meter
      pupilDiameterLeft = eyeData.verbose_data.left.pupil_diameter_mm;
      pupilDiameterRight = eyeData.verbose_data.right.pupil_diameter_mm;
      pupilDiameterCombined = eyeData.verbose_data.combined.eye_data.pupil_diameter_mm;
      Debug.Log("pupilDiameterLeft: " + pupilDiameterLeft);

      // A value representing how open the eye is in [0,1]
      eyeOpenLeft = eyeData.verbose_data.left.eye_openness;
      eyeOpenRight = eyeData.verbose_data.right.eye_openness;
      eyeOpenCombined = eyeData.verbose_data.combined.eye_data.eye_openness;
      Debug.Log("eyeOpenLeft: " + eyeOpenLeft);

      // The normalized position of a pupil in [0,1]
      pupilPositionLeft = eyeData.verbose_data.left.pupil_position_in_sensor_area;
      pupilPositionRight = eyeData.verbose_data.right.pupil_position_in_sensor_area;
      pupilPositionCombined = eyeData.verbose_data.combined.eye_data.pupil_position_in_sensor_area;
      Debug.Log("pupilPositionLeft: " + pupilPositionLeft);

      lock (DebugWriter)
      {
          CSVwriter.Write();
      }

    }

    
    /// <summary>
    /// Terminates an anipal module
    /// </summary>
    private void Release()
    {
      if (eye_callback_registered == true)
      {
          SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
          eye_callback_registered = false;
      }
    }
    

  }
}
