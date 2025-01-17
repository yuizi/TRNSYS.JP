#include <cmath>
#include <fstream>
#include "TRNSYS.h" //TRNSYS acess functions (allow to acess TIME etc.) 
//************************************************************************
// Object: Noname
// IISiBat Model: Type201
// 
// Author: 
// Editor: 
// Date:	 July 14, 2015 last modified: July 14, 2015
// 
// 
// *** 
// *** Model Parameters 
// *** 
//			Mult	- [-Inf;+Inf]

// *** 
// *** Model Inputs 
// *** 
//			Inp1	- [-Inf;+Inf]
//			Inp2	- [-Inf;+Inf]

// *** 
// *** Model Outputs 
// *** 
//			Out1	- [-Inf;+Inf]

// *** 
// *** Model Derivatives 
// *** 

// (Comments and routine interface generated by TRNSYS Studio)
//************************************************************************

//
extern "C" __declspec(dllexport) 
int TYPE201           (
             double &time,  // the simulation time
             double xin[],  // the array containing the component InpUTS
             double xout[], // the array which the component fills with its appropriate OUTPUTS
             double &t,     // the array containing the dependent variables for which the derivatives are evaluated 
             double &dtdt,  // the array containing the derivatives of T which are evaluated 
             double par[],  // the array containing the PARAMETERS of the component
             int info[],    // the information array described in Section 3.3.3 of the manual
             int icntrl     // the control array described in Section 3.3.4 of the manual
            )
{
  //*************************************************************************
  //*** TYPE implementation
  //*** This function will be called by TRNSYS 
  //*** - once at the beginning of the simulation for initialization
  //*** - once at the beginning of every timestep for initialization
  //*** - once for each iteration of the TRNSYS solver
  //*** - once at the end of each timestep for cleanup
  //*** - once at the end of the simulation for cleanup
  //*************************************************************************
  // 
  //***
  //*** WARNING: explanations in the TRNSYS manual use FORTRAN conventions for 
  //***          array indices. Subtract 1 to obtain 0-based C or C++ conventions. 
  //*** Example: 
  //***          TRNSYS manual: info(6) = number of OUTPUTS 
  //***          -> write no = info[5] to obtain number of outputs in C or C++
  //***
  //*** We also spell variables in lower case according to C tradition, while they
  //*** are spelled in uppercase in the TRNSYS manual (according to FORTRAN tradition)
  //*** 


//-----------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------

  // *** STANDARD TRNSYS DECLARATIONS
  int npar= 1;   // number of parameters we expect
  int nin= 2;   // number of inputs
  int nout=1; // number of outputs
  int nder=0;   // number of derivatives
  int iunit; // UNIT number ('serial number' of the component, from the input file (the 'deck')
  int itype; // TYPE number (component number) 

  int iterationmode = 1; //An indicator for the iteration mode (default=1)
  int xin01 = 1; // no. for input-1
  int xin02 = 2; // no. for input-2
  int xout01 = 1; // no. for output-1

  double val = 0.0;


  // read context information from TRNSYS
  // (uncomment lines as required)
	//info[5] = nout;  // number of outputs 
	//iunit = info[0]; // UNIT number
	//itype = info[1]; // TYPE number
	setNumberofOutputs(&nout);
	iunit = getCurrentUnit();
	itype = getCurrentType();
	//info[2]	; // number of INPUTS specified by the user of the component 
	//info[3]	; // number of PARAMETERS specified by the user of the component
	//info[4]	; // number of DERIVATIVES specified by the user of the component
	//info[5]	; // number of OUTPUTS specified by the user of the component

  //info[6]	; // number of iterative calls to the UNIT in the current timestep
              // -2 = initialization
              // -1	= initial call in simulation for this UNIT
	            //  0 = first call in timestep for this UNIT.
              //  1	= second call in timestep for this UNIT, etc.

	//info(7)	; // total number of calls to the UNIT in the simulation
  // *** inform TRNSYS about properties of this type
	//info[8] = 0; // indicates whether TYPE depends on the passage of time: 0=no
	//info[9]	= 0; //	use to allocate storage (see Section 3.5 of the TRNSYS manual): 0 = none
	setIterationMode(&iterationmode);
	// info[10]; // indicates number of discrete control variables (see Section 3.3.4 of the TRNSYS manual)
//-----------------------------------------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------------------------------------
//    ADD DECLARATIONS AND DEFINITIONS FOR THE USER-VARIABLES HERE

//-----------------------------------------------------------------------------------------------------------------------

//    PARAMETERS
      double Mult;

//    INPUTS
      double Inp1;
      double Inp2;

//-----------------------------------------------------------------------------------------------------------------------
//       READ IN THE VALUES OF THE PARAMETERS IN SEQUENTIAL ORDER
      //Mult=par[0];
	  int pn = 1;
	  Mult = getParameterValue(&pn);

//-----------------------------------------------------------------------------------------------------------------------
//    RETRIEVE THE CURRENT VALUES OF THE INPUTS TO THIS MODEL FROM THE XIN ARRAY IN SEQUENTIAL ORDER

      //Inp1=xin[0];
      //Inp2=xin[1];
	 //iunit=info[0];
	 //itype=info[1];

	  Inp1 = getInputValue(&xin01);
	  Inp2 = getInputValue(&xin02);
	  iunit = getCurrentUnit();
	  itype = getCurrentType();

//-----------------------------------------------------------------------------------------------------------------------
//    SET THE VERSION INFORMATION FOR TRNSYS
    //  if (info[6]== -2) 
    //{
	   //info[11]=16;
    // // add additional initialisation code here, if any
	   //return 1;
    //}
	  if (getIsVersionSigningTime())
	  {
		  int ver = 17;
		  setTypeVersion(&ver);
		  return 1;
	  }
//-----------------------------------------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------------------------------------
//    DO ALL THE VERY LAST CALL OF THE SIMULATION MANIPULATIONS HERE
	//  if (info[7]== -1) 
	   //return 1;
	  if (getIsLastCallofSimulation())
	  {
		  return 1;
	  }
//-----------------------------------------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------------------------------------
//    PERFORM ANY 'AFTER-ITERATION' MANIPULATIONS THAT ARE REQUIRED HERE
//    e.g. save variables to storage array for the next timestep
//      if (info[12]>0) 
//      {
////	   nitems=0;
////	   stored[0]=... (if NITEMS > 0)
////        setStorageVars(STORED,NITEMS,INFO)
//	     return 1;
//      }
	  if (getIsEndOfTimestep())
	  {
		  return 1;
	  }
//-----------------------------------------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------------------------------------
//    DO ALL THE VERY FIRST CALL OF THE SIMULATION MANIPULATIONS HERE
//      if (info[6]== -1) // first call of this component in the simulation
//      {
////       SET SOME INFO ARRAY VARIABLES TO TELL THE TRNSYS ENGINE HOW THIS TYPE IS TO WORK
//         info[5]=nout;				
//         info[8]=1;				
//	     info[9]=0;	// STORAGE FOR VERSION 16 HAS BEEN CHANGED				
//
////       SET THE REQUIRED NUMBER OF INPUTS, PARAMETERS AND DERIVATIVES THAT THE USER SHOULD SUPPLY IN THE INPUT FILE
////       IN SOME CASES, THE NUMBER OF VARIABLES MAY DEPEND ON THE VALUE OF PARAMETERS TO THIS MODEL....
//         nin=2;
//	     npar=1;
//	     nder=0;
//	       
////       CALL THE TYPE CHECK SUBROUTINE TO COMPARE WHAT THIS COMPONENT REQUIRES TO WHAT IS SUPPLIED IN 
////       THE TRNSYS INPUT FILE
//    int dummy=1;
//		TYPECK(&dummy,info,&nin,&npar,&nder);
//
////       SET THE NUMBER OF STORAGE SPOTS NEEDED FOR THIS COMPONENT
////         nitems=0;
////	     setStorageSize(nitems,info)
//
////       RETURN TO THE CALLING PROGRAM
//         return 1;
//      }

	  if (getIsFirstCallfSimulation())
	  {
		  setNumberofParameters(&npar);          //The number of parameters that the the model wants
		  setNumberofInputs(&nin);               //The number of inputs that the the model wants
		  setNumberofDerivatives(&nder);          //The number of derivatives that the the model wants
		  setNumberofOutputs(&nout);          //The number of outputs that the the model produces
		  setIterationMode(&iterationmode);				//An indicator for the iteration mode (default=1).  Refer to section 8.4.3.5 of the documentation for more details.
		  //setNumberStoredVariables(&nstatic, &ndynamic); //The number of static variables that the model wants stored in the global storage array and the number of dynamic variables that the model wants stored in the global storage array
		  setNumberofDiscreteControls(&nder);               //The number of discrete control functions set by this model (a value greater than zero requires the user to use Solver 1: Powell's method)

		  return 1;
	  }


//-----------------------------------------------------------------------------------------------------------------------
//    DO ALL OF THE INITIAL TIMESTEP MANIPULATIONS HERE - THERE ARE NO ITERATIONS AT THE INTIAL TIME
      //if (time < (getSimulationStartTime() +
      // getSimulationTimeStep()/2.0)) 
      // {
	  if (getIsStartTime())
	  {


//       SET THE UNIT NUMBER FOR FUTURE CALLS
         //iunit=info[0];
         //itype=info[1];
		 iunit = getCurrentUnit();
		 itype = getCurrentType();

//       CHECK THE PARAMETERS FOR PROBLEMS AND RETURN FROM THE SUBROUTINE IF AN ERROR IS FOUND
//         if(...) TYPECK(-4,INFO,0,"BAD PARAMETER #",0)

//       PERFORM ANY REQUIRED CALCULATIONS TO SET THE INITIAL VALUES OF THE OUTPUTS HERE
//		 Out1
			//xout[0]=0;
			val = 0.0;
			setOutputValue(&xout01, &val);

//       PERFORM ANY REQUIRED CALCULATIONS TO SET THE INITIAL STORAGE VARIABLES HERE
//         nitems=0;
//	   stored[0]=...

//       PUT THE STORED ARRAY IN THE GLOBAL STORED ARRAY
//         setStorageVars(stored,nitems,info)

//       RETURN TO THE CALLING PROGRAM
         return 1;

      }
//-----------------------------------------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------------------------------------
//    *** ITS AN ITERATIVE CALL TO THIS COMPONENT ***
//-----------------------------------------------------------------------------------------------------------------------

	    
//-----------------------------------------------------------------------------------------------------------------------
//    RETRIEVE THE VALUES IN THE STORAGE ARRAY FOR THIS ITERATION
//      nitems=
//	    getStorageVars(stored,nitems,info)
//      stored[0]=
//-----------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------
//    CHECK THE INPUTS FOR PROBLEMS
//      if(...) TYPECK(-3,INFO,'BAD INPUT #',0,0)
//	if(IERROR.GT.0) RETURN 1
//-----------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------
//    *** PERFORM ALL THE CALCULATION HERE FOR THIS MODEL. ***
//-----------------------------------------------------------------------------------------------------------------------

//		ADD YOUR COMPONENT EQUATIONS HERE; BASICALLY THE EQUATIONS THAT WILL
//		CALCULATE THE OUTPUTS BASED ON THE PARAMETERS AND THE INPUTS.	REFER TO
//		CHAPTER 3 OF THE TRNSYS VOLUME 1 MANUAL FOR DETAILED INFORMATION ON
//		WRITING TRNSYS COMPONENTS.













//-----------------------------------------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------
//    SET THE STORAGE ARRAY AT THE END OF THIS ITERATION IF NECESSARY
//      nitmes=
//      stored(1)=
//	    setStorageVars(STORED,NITEMS,INFO)
//-----------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------
//    REPORT ANY PROBLEMS THAT HAVE BEEN FOUND USING CALLS LIKE THIS:
//      MESSAGES(-1,'put your message here','MESSAGE',IUNIT,ITYPE)
//      MESSAGES(-1,'put your message here','WARNING',IUNIT,ITYPE)
//      MESSAGES(-1,'put your message here','SEVERE',IUNIT,ITYPE)
//      MESSAGES(-1,'put your message here','FATAL',IUNIT,ITYPE)
//-----------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------
//    SET THE OUTPUTS FROM THIS MODEL IN SEQUENTIAL ORDER AND GET OUT

//		 Out1
			//xout[0] = (Inp1 + Inp2) * Mult;
			val = (Inp1 + Inp2) * Mult;
			setOutputValue(&xout01, &val);

//-----------------------------------------------------------------------------------------------------------------------
//    EVERYTHING IS DONE - RETURN FROM THIS SUBROUTINE AND MOVE ON
      return 1;
      }
//-----------------------------------------------------------------------------------------------------------------------
