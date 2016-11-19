
MergeServices.factory('MergeManager', ['$http', function ($http) {

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
    };
     
    return {
        merge: merge
    };
}]);