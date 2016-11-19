﻿
MergeServices.factory('MergeManager', ['$http', function ($http) {

  //  var summary = {};
 //   summary.matched = 0;
  //  summary.unmatched = 0;
  //  summary.merge = "unavailable";

    var merge = function (vcFileName, vcFileType, apFileName, apFileType, qbFileName, qbFileType) {
        return $http.get(server + "/api/merge",
            {
                params:
                   {
                       "vcFileName": vcFileName,
                       "vcFileType": vcFileType,
                       "apFileName": apFileName,
                       "apFileType": apFileType,
                       "qbFileName": qbFileName,
                       "qbFileType": qbFileType
                   }
            }).then(function (result) {
                return result.data;
            });
    
            /*
            if (desktop == true) {
                return $http.get("http://localhost/msm/api/merge",
                     {
                         params:
                            {
                                "vcFileName": vcFileName,
                                "vcFileType": vcFileType,
                                "apFileName": apFileName,
                                "apFileType": apFileType,
                                "qbFileName": qbFileName,
                                "qbFileType": qbFileType
                            }
                     }).then(function (result) {
                         return result.data;
                     });
            } else {
                return $http.get("https://mymsm.apphb.com/api/merge",
                     {
                         params:
                            {
                                "vcFileName": vcFileName,
                                "vcFileType": vcFileType,
                                "apFileName": apFileName,
                                "apFileType": apFileType,
                                "qbFileName": qbFileName,
                                "qbFileType": qbFileType
                            }
                     }).then(function (result) {
                         return result.data;
                     });
            }
            */
    };
     
    return {
        merge: merge
    };
}]);