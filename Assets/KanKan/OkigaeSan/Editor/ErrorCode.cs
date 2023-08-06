using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkigaeSan
{
    enum ErrorCode
    {
        NoError,
        NotFoundVRCScript,
        NotFoundBaseAnimationController,
        NotFoundAddtiveAnimationController,
        NotFoundGestureAnimationController,
        NotFoundActionAnimationController,
        NotFoundFXAnimationController,
        DetecedSDKChange,
        NotFoundExpressionMenu,
        NotFoundExpressionParameter,
    }
}