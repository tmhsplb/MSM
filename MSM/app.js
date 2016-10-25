
var debugging = true;

var FileServices = angular.module('FileServices', ['ngResource']);
var MergeServices = angular.module('MergeServices', ['ngResource']);

//angular.module('showcase', ['datatables']);

//var MSMApp = angular.module('MSMApp', ['ngRoute', 'FileServices', 'datatables']);

// var MSMApp = angular.module('MSMApp', ['ngRoute', 'FileServices', 'datatables', 'showcase.withPromise']);

//var MSMApp = angular.module('MSMApp', ['ngRoute', 'FileServices', 'datatables', 'linqtoexcel']);

var MSMApp = angular.module('MSMApp', ['ngRoute', 'FileServices', 'MergeServices', 'datatables', 'datatables.bootstrap', 'datatables.buttons']);
   
 
    


