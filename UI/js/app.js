
var app = angular.module('appint',
    ['ui.bootstrap', 'angular-json-editor', 'ngSanitize', 'ui.select', 'jsTree.directive',
        'ui.ace', 'ngFileReader', 'ngFileSaver', 'angularScreenfull']);

//var host = 'http://localhost:9229/appint/api';
var host = 'http://localhost:61081/api';

//*************************************AppIntCtrl******************************
app.controller('AppIntCtrl', function ($scope, $uibModal, $http, $log) {

    //****Tools*****
    $scope.tools = [
        {
            name: 'Create project',
            icon: 'glyphicon glyphicon-plus',
            src: 'create-project.html',
            ctrl: 'ProjectCtrl',
            size: 'sm'
        },
        {
            name: 'Entity editor',
            icon: 'glyphicon glyphicon-list-alt',
            src: 'entity-editor.html',
            ctrl: 'EntityCtrl'
        },
        {
            name: 'Create service',
            icon: 'fa fa-globe',
            src: 'create-service.html',
            ctrl: 'ServiceCtrl',
            size: 'lg'
        },
        {
            name: 'File explorer',
            icon: 'fa fa-folder-o',
            src: 'file-explorer.html',
            ctrl: 'FileCtrl',
            size: 'lg'
        },
        {
            name: 'Library manager',
            icon: 'glyphicon glyphicon-book',
            src: 'file-explorer.html',
            ctrl: 'FileCtrl',
            size: 'lg',
            param: { name: 'lib' }
        },
        {
            name: 'Rest client',
            icon: 'fa fa-globe',
            src: 'file-explorer.html',
            ctrl: 'FileCtrl',
            size: 'lg'
        },
        {
            name: 'data manager',
            icon: 'fa fa-database',
            src: 'db-manager.html',
            ctrl: 'ProjectCtrl',
            size: 'md'
        },
        {
            name: 'create utility',
            icon: 'fa fa-suitcase',
            src: 'code-editor.html',
            ctrl: 'FileCtrl',
            size: 'lg',
            param: {name: 'utility'}
        },
        {
            name: 'iis explorer',
            icon: 'fa fa-suitcase',
            src: 'file-explorer.html',
            ctrl: 'FileCtrl',
            size: 'lg',
            param: { name:'iis', service: host + '/server' }
        }
    ];

    $scope.showDialog = function (tool) {
        if (tool.size == undefined) tool.size = 'md';
        var modalInstance = $uibModal.open({
            templateUrl: tool.src,
            controller: tool.ctrl,
            size: tool.size,
            resolve: {
                param: tool.param
            }
        });
    }

    //****Projects****
    $scope.projects = [];

    var uri = host + '/Project/';

    $http.get(uri, config).then(getProjectsSuccess, errorCallback);
    
    function getProjectsSuccess(data) {
        angular.forEach(data.data, function (value) {
            $scope.projects.push({ name: value, icon: 'l l-' + value.substr(0, 1).toLowerCase() });
        });
    }
    
    $scope.showProjectExplorer = function (id) {
        var modalInstance = $uibModal.open({
            templateUrl: 'project-explorer.html',
            controller: 'ProjectCtrl',
            size: 'lg',
            resolve:{param: function () {
                return { 'project': id };
            }}
        });
    }
});

//*************************************ProjectCtrl*****************************
app.controller('ProjectCtrl', function ($scope, $http, $uibModalInstance, $log, param, $uibModal) {
    $scope.server = 'mssql';
    var uri = host + '/Project/';

    if (param != undefined) $scope.projectName = param.project;

    $scope.createProject = function () {
        var data = new String($scope.projectName);
        $http.post(uri, data, config).then(projectCreated, errorCallback);  
    }

    function projectCreated(data) {
        alert(data.data);
        $uibModalInstance.dismiss();
    }

    $scope.cancel = function () {
        $uibModalInstance.dismiss();
    };
    

    //****Entity->Project****
    $scope.projects = [];
    
    $scope.getProjects = function () {
        $http.get(uri, config).then(getProjectsSuccess, errorCallback);
    }

    function getProjectsSuccess(data){
        $scope.projects = data.data;
    }

    $scope.project = {};
    $scope.project.selected = undefined;

    $scope.projectSelected = function () {
        $uibModalInstance.close($scope.project.selected);
    }

    $scope.getProjectDetail = function () {
        $http.get(uri + $scope.projectName, config).then(getProjectDetailSuccess, errorCallback);
    }

    function getProjectDetailSuccess(res) {
        $scope.services = res.data.services;
        $scope.entities = res.data.entities;
        $scope.utilities = res.data.utilities;
    }

    $scope.getCode = function (id) {
        $http.get(host + '/File/Resource?resource=' + id, config).then(showCode, errorCallback);
    }

    function showCode(data) {
        var modalInstance = $uibModal.open({
            templateUrl: "code-editor.html",
            size: "lg",
            controller: "FileCtrl",
            resolve: {
                param: function () {
                    return { 'code': data.data };
                }
            }
        });

        //Updating code from Code Viewer
        modalInstance.result.then(function (code) {
            $scope.code = code;
        });
    };
});

//*************************************EntityCtrl******************************
app.controller('EntityCtrl', function ($scope, $http, $uibModalInstance, $uibModal, FileSaver, $log) {

    var uri =  host + '/Entity/';
    
    $scope.project = undefined;

    $scope.createEntity = function () {
        var data = {};
        data.Name = $scope.entityName;
        data.Json = JSON.stringify($scope.json);
        data.Project = $scope.project
        $http.post(uri, data, config).then(entityCreated, errorCallback);
    }

    function entityCreated(data) {
        alert(data.data);
        $uibModalInstance.dismiss();
    }

    $scope.cancel = function () {
        $uibModalInstance.dismiss();
    };


    $scope.options = {
        "mode": "code",
        "modes": ['tree', 'form', 'code', 'text'],
        "history": false
    };
    
    $scope.showDialog = function () {
        var modalInstance = $uibModal.open({
            templateUrl: "choose-project.html",            
            size: "sm",
            controller: "ProjectCtrl",
            resolve: {
                param: undefined
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.project = selectedItem;
        });
    };
    
    $scope.projects = [];

    $scope.getProjects = function () {
        var uri = host + '/Project/';
        $http.get(uri, config).then(getProjectsSuccess, errorCallback);
    }

    function getProjectsSuccess(data) {
        $scope.projects = data.data;
    }


    $scope.getEntities = function (project) {
        if (project == undefined) project = 'Global';
        $http.get(uri + "?project=" + project , config).then(getEntitiesSuccess, errorCallback);
    }

    function getEntitiesSuccess(data) {
        $scope.entities = [];
        angular.forEach(data.data, function (value) {
            $scope.entities.push(value.Name);
        });
    }

    $scope.entity = {};
    $scope.entity.selected = undefined;

    $scope.entitySelected = function () {
        $uibModalInstance.close($scope.entity.selected);
    }

    angular.extend($scope, {

        readMethod: "readAsText",

        onReaded: function (e, file) {
            $scope.json = JSON.parse(e.target.result);
        }
    });

    $scope.download = function () {
        var blob = new Blob([JSON.stringify($scope.json)], { type: 'application/json;charset=utf-8' });
        FileSaver.saveAs(blob, ($scope.entityName == undefined ? 'document' : $scope.entityName) + '.json');
    }
});

//*************************************ServiceCtrl******************************
app.controller('ServiceCtrl', function ($scope, $uibModal, $filter, $http, $log) {

    $scope.service = {};
    $scope.service.project = undefined;
    $scope.service.methods = [{ name: 'Get', enabled: false }, { name: 'Post', enabled: false }, { name: 'Put', enabled: false }, { name: 'Delete', enabled: false }];

    $scope.entity = undefined;
    
    var uri = host + '/Service/';

    $scope.generateCode = function () {
        $scope.service.project = $scope.project == undefined ? 'Global' : $scope.project;
        var data = $scope.service;
        var methods = $filter('filter')($scope.service.methods, { enabled: true });
        data.methods = methods;
        console.log(JSON.stringify(data));
        $http.post(uri, data, config).then(codeGenerated, errorCallback);
    }

    function codeGenerated(data) {
        alert(data.data);
        $scope.code = data.data;
    }

    $scope.cancel = function () {
        $uibModal.dismiss();
    };

    $scope.showDialog = function () {
        var modalInstance = $uibModal.open({
            templateUrl: "choose-project.html",
            size: "sm",
            controller: "ProjectCtrl",
            resolve: {
                param: undefined
            }
        });
        modalInstance.result.then(function (selectedItem) {
            $scope.project = selectedItem;
        });
    };

    $scope.showEntity = function () {
        var modalInstance = $uibModal.open({
            templateUrl: "choose-entity.html",
            size: "sm",
            controller: "EntityCtrl"
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.entity = selectedItem;
            
        });
    };

    $scope.addReqParam = function (index) {
        if ($scope.service.methods[index].request == undefined) $scope.service.methods[index].request = {};
        if ($scope.service.methods[index].request.uriParameters == undefined) $scope.service.methods[index].request.uriParameters = [];
        $scope.service.methods[index].request.uriParameters.push({ name: $scope.service.methods[index].param, type: $scope.service.methods[index].type });
        $scope.service.methods[index].param = undefined;
        $scope.service.methods[index].type = undefined;
    }

    $scope.removeReqParam = function (index, modelIndex) {
        $scope.methods[modelIndex].reqParams.splice(index, 1);
    }
    
    $scope.showCode = function () {
        var modalInstance = $uibModal.open({
            templateUrl: "code-editor.html",
            size: "lg",
            controller: "FileCtrl",
            resolve: {
                param: function () {
                    return { 'code': $scope.code };
                }
            }
        });

        //Updating code from Code Viewer
        modalInstance.result.then(function (code) {
            $scope.code = code;
        });
    };

    $scope.createService = function () {
        $scope.service.project = $scope.project == undefined ? 'Global' : $scope.project;
        $scope.service.content = $scope.code;
        var data = $scope.service;
        data.methods = {};
        console.log(JSON.stringify(data));
        $http.post(uri + 'Create/', data, config).then(serviceCreated, errorCallback);
    }

    function serviceCreated(data) {
         
    }
});


//*************************************FileCtrl*********************************
app.controller('FileCtrl', function ($scope, $http, param, $uibModalInstance, $uibModal) {

    $scope.sites = [];

    //param flow is from Service -> Code Viewer
    if (param != undefined) {

        if (param.code != undefined) $scope.fileViewer = param.code;
        if (param.service != undefined) $scope.service = param.service;
        if (param.name != undefined) $scope.name = param.name;
    }
    else {
        $scope.service = host + '/file/';
        $scope.fileViewer = 'Please select a file to view its contents';
    }

    $scope.save = function () {
        $uibModalInstance.close($scope.fileViewer);
    }

    $scope.tree_core = {

        multiple: false,  // disable multiple node selection

        check_callback: function (operation, node, node_parent, node_position, more) {
            // operation can be 'create_node', 'rename_node', 'delete_node', 'move_node' or 'copy_node'
            // in case of 'rename_node' node_position is filled with the new node name

            if (operation === 'move_node') {
                return false;   // disallow all dnd operations
            }
            return true;  // allow all other operations
        }
    };

    $scope.mode = "csharp";

    $scope.nodeSelected = function (e, data) {
        var _l = data.node.li_attr;
        if (_l.isLeaf) {
            $scope.mode = getMode(_l.base);
            $scope.modeChanged();
            $http.get( host + '/file/resource?resource=' + encodeURIComponent(_l.base))
            .then(function (data) {
                var _d = data.data;
                if (typeof _d == 'object') {
                    //http://stackoverflow.com/a/7220510/1015046//
                    _d = JSON.stringify(_d, undefined, 2);
                }
                $scope.fileViewer = _d;
            });
        }
        else {
            //http://jimhoskins.com/2012/12/17/angularjs-and-apply.html//
            $scope.$apply(function () {
                $scope.fileViewer = 'Please select a file to view its contents';
            });
        }
    };
    
    $scope.aceOption = {
        mode: $scope.mode.toLowerCase(),
        onLoad: function (_ace) {
            //The ace instance in the scope...
            $scope.modeChanged = function () {
                _ace.getSession().setMode("ace/mode/" + $scope.mode.toLowerCase());
            };

        }
    };

    function getMode(file) {
        var fileExt = file.split('.').pop();
        if (fileExt.toLowerCase() == 'js') return 'javascript';
        if (fileExt.toLowerCase() == 'cs') return 'csharp';
        if (fileExt.toLowerCase() == 'config' || fileExt.toLowerCase() == 'csproj') return 'xml';
        return fileExt;
    }

    $scope.showDialog = function () {
        var modalInstance = $uibModal.open({
            templateUrl: "choose-project.html",
            size: "sm",
            controller: "ProjectCtrl",
            resolve: {
                param: undefined
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.project = selectedItem;
        });
    };

    $scope.projects = [];

    $scope.getProjects = function () {
        var uri = host + '/Project/';
        $http.get(uri, config).then(getProjectsSuccess, errorCallback);
    }

    function getProjectsSuccess(res) {
        $scope.projects = res.data;
    }

    $scope.createFile = function () {
        var data = {};
        data.Name = $scope.fileName;
        data.Content = $scope.fileViewer;
        data.Project = $scope.project
        alert(JSON.stringify(data));
        $http.post(host + '/Utility/', data, config).then(fileCreated, errorCallback);
    }

    function fileCreated(res) {
        alert(res.data);
        $uibModalInstance.dismiss();
    }
});


//*************************************Common***********************************
var config = {
    headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
    }
};

function errorCallback(response) {
    alert(response.data.Message);
}

//https://github.com/angular-ui/ui-ace
//http://angular-ui.github.io/ui-ace/
//https://github.com/SparrowJang/ngFileReader
//http://www.sparrowjang.com/ngFileReader/example/index.html
//https://github.com/alferov/angular-file-saver
//http://alferov.github.io/angular-file-saver/

//https://developer.github.com/guides/getting-started/
//http://examples.notsoclever.cc/angular_github/
//https://github.com/octokit/octokit.net
//https://github.com/libgit2/libgit2sharp
//https://github.com/hrajchert/angular-screenfull
//http://hrajchert.github.io/angular-screenfull/