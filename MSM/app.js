
var desktop = true;
var server;

if (desktop == true) {
    server = "http://localhost/msm/";
} else {
    server = "https://mymsm.apphb.com/";
}


var FileServices = angular.module('FileServices', ['ngResource']);
var MergeServices = angular.module('MergeServices', ['ngResource']);
var ResearchServices = angular.module('ResearchServices', ['ngResource']);

//angular.module('showcase', ['datatables']);

//var MSMApp = angular.module('MSMApp', ['ngRoute', 'FileServices', 'datatables']);

// var MSMApp = angular.module('MSMApp', ['ngRoute', 'FileServices', 'datatables', 'showcase.withPromise']);

//var MSMApp = angular.module('MSMApp', ['ngRoute', 'FileServices', 'datatables', 'linqtoexcel']);

var MSMApp = angular.module('MSMApp', ['ngRoute', 'ngSanitize', 'FileServices', 'MergeServices', 'ResearchServices', 'datatables', 'datatables.bootstrap', 'datatables.buttons']);
   
 
    


