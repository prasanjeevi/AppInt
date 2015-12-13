var host_iis = 'http://localhost:9229/appint/api';  //For IIS
var host = 'http://localhost:61081/api';      //For debuging

var app = angular.module('appint',
    ['ui.bootstrap', 'angular-json-editor', 'ngSanitize', 'ui.select', 'jsTree.directive',
        'ui.ace', 'ngFileReader', 'ngFileSaver', 'angularScreenfull']);

app.factory("prompt", function ($window, $q) {

    // Define promise-based prompt() method.
	function prompt(message, defaultValue) {
	    var defer = $q.defer();

		// The native prompt will return null or a string.
		var response = $window.prompt(message, defaultValue);

		if (response === null) {
			defer.reject();
		} else {
			defer.resolve(response);
		}

		return (defer.promise);
	}

	return (prompt);
});

//*************************************AppIntCtrl******************************
app.controller('AppIntCtrl', function ($scope, $http, $uibModal) {

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
            name: 'create utility',
            icon: 'fa fa-suitcase',
            src: 'code-editor.html',
            ctrl: 'FileCtrl',
            size: 'lg',
            param: { name: 'utility', title:'Utility Code Editor' }
        },
        {
            name: 'data manager',
            icon: 'fa fa-database',
            src: 'db-manager.html',
            ctrl: 'ProjectCtrl',
            size: 'md'
        },
        {
            name: 'File explorer',
            icon: 'fa fa-folder-o',
            src: 'file-explorer.html',
            ctrl: 'FileCtrl',
            size: 'lg'
        },
        {
            name: 'Library explorer',
            icon: 'glyphicon glyphicon-book',
            src: 'file-explorer.html',
            ctrl: 'FileCtrl',
            size: 'lg',
            param: { name: 'lib', service: host + '/file/lib', title:'Library explorer' }
        },
        {
            name: 'Rest client',
            icon: 'fa fa-globe',
            src: 'rest-client.html',
            ctrl: 'RestCtrl',
            size: 'lg'
        },
        {
            name: 'iis explorer',
            icon: 'fa fa-suitcase',
            src: 'file-explorer.html',
            ctrl: 'FileCtrl',
            size: 'lg',
            param: { name: 'iis', service: host_iis + '/server', title: 'IIS explorer' }
        }
    ];

    //App -> Tools
    $scope.showTool = function (tool) {
        var modalInstance = $uibModal.open({
            templateUrl: tool.src,
            controller: tool.ctrl,
            size: tool.size || 'md',
            resolve: {
                param: tool.param
            }
        });

        if (tool.src == 'create-project.html') {
            //on -> createProjectSuccess -> Refresh projects
            modalInstance.result.then(function (project) {
                $scope.projects.push({ name: project, icon: 'l l-' + project.substr(0, 1).toLowerCase() });
            });
        }
    }


    //****Projects****
    $scope.projects = [];

    var uri = host + '/Project';

    $http.get(uri, config).then(getProjectsSuccess, errorCallback);
    
    function getProjectsSuccess(res) {
        angular.forEach(res.data, function (value) {
            $scope.projects.push({ name: value, icon: 'l l-' + value.substr(0, 1).toLowerCase() });
        });
    }
    
    //App -> Project Explorer
    $scope.showProjectExplorer = function (project){
        var modalInstance = $uibModal.open({
            templateUrl: 'project-explorer.html',
            controller: 'ProjectCtrl',
            size: 'lg',
            resolve: {
                param: function () {
                    return { 'project': project };
                }
            }
        });
    }
});

//*************************************ProjectCtrl*****************************
app.controller('ProjectCtrl', function ($scope, $http, $uibModal, $uibModalInstance, prompt, param) {
    
    var uri = host + '/Project';
    $scope.loader = false;
    //param from Project Explorer Flow
    if (param != undefined) $scope.project = param.project;
        
    $scope.createProject = function () {
        var data = new String($scope.project);
        $scope.loader = true;
        $http.post(uri, data, config).then(createProjectSuccess, errorCallback);
    }

    function createProjectSuccess(res) {
        $scope.loader = false;
        alert(res.data);
        $uibModalInstance.close($scope.project); //To -> AppIntCtrl
    }

    $scope.cancel = function () {
        $uibModalInstance.dismiss();
    };
    

    //****Entity->Project****
    $scope.projects = [];
    
    $scope.getProjects = function () {
        $http.get(uri, config).then(getProjectsSuccess, errorCallback);
    }

    function getProjectsSuccess(res){
        $scope.projects = res.data;
    }

    $scope.projectSelected = function (project) {
        $uibModalInstance.close(project);
    }


    //****ProjectExplorer****
    $scope.getProjectDetail = function () {
        $http.get(uri + "/" + $scope.project, config).then(getProjectDetailSuccess, errorCallback);
    }

    function getProjectDetailSuccess(res) {
        $scope.services = res.data.services;
        $scope.entities = res.data.entities;
        $scope.utilities = res.data.utilities;
        $scope.dals = res.data.dal;
        $scope.configs = res.data.configs;
    }

    //ProjectExplorer -> Show Code
    $scope.showCode = function (id) {
        $http.get(host + '/File/Resource?resource=' + id, config).then(showCodeSuccess, errorCallback);
    }

    function showCodeSuccess(res) {
        var modalInstance = $uibModal.open({
            templateUrl: "code-editor.html",
            size: "lg",
            controller: "FileCtrl",
            resolve: {
                param: function () {
                    return { 'code': res.data, 'uri': res.config.url };
                }
            }
        });

        //Updating code from Code Viewer
        modalInstance.result.then(function (code) {
            $scope.code = code;
        });
    };

    $scope.showTool = function (src, ctrl, size, project) {

        var modalInstance = $uibModal.open({
            templateUrl: src,
            controller: ctrl,
            size: size,
            resolve: {
                param: function () {
                    if (ctrl == 'FileCtrl')
                        return { 'name': 'utility', 'title': 'Utility Code Editor', 'project': project };
                    else
                        return { 'project': project};
                }
            }
        });
    }

    //Data Manager
    $scope.serverType = 'mssql';
    $scope.port = 1433;
    $scope.dbContext = $scope.project == undefined ? 'GlobalContext' : $scope.project + 'Context';

    $scope.setDBContext = function (project) {
        $scope.project = project;
        $scope.dbContext = project + 'Context';
        $scope.dbs = ["Create DB " + project];
    }
    $scope.db = {};
    
    $scope.getDBs = function () {
        var data = {};
        data.Name = $scope.dbContext;
        data.Server = $scope.server;
        data.Port = $scope.port;
        data.Username = $scope.username;
        data.Password = $scope.password;
        data.Project = $scope.project;
        data.Database = (new String($scope.db.selected)).indexOf('Create DB ') == -1 ? $scope.db.selected : 'master';

        $scope.loaderSub = true;
        $http.post(host + '/Data/TestConnection', data, config).then(getDBsSuccess, errorCallback);
    }

    function getDBsSuccess(res) {
        $scope.loaderSub = true;
        $scope.dbs = ["Create DB " + $scope.project].concat(res.data);
    }

    $scope.saveDB = function () {
        var data = {};
        data.Name = $scope.dbContext;
        data.Server = $scope.server;
        data.Port = $scope.port;
        data.Username = $scope.username;
        data.Password = $scope.password;
        data.Project = $scope.project;
        
        var db = new String($scope.db.selected);
        data.Database = db.replace('Create DB ', '');

        $scope.loader = true;
        $http.post(host + '/Data/SaveConnection', data, config).then(saveDBSuccess, errorCallback);
    }

    function saveDBSuccess(res) {
        $scope.loader = false;
        alert(res.data);
        $uibModalInstance.dismiss();
    }

    //PreBuild & Build
    $scope.preBuild = function () {
        $scope.message = 'Processing pre build';
        $scope.loader = true;
        var data = new String($scope.project);
        $http.post(host + '/Project/PreBuild', data, config).then(successCallback, errorCallback);
    }
    
    $scope.build = function () {
        $scope.message = 'Processing build';
        $scope.loader = true;
        var data = new String($scope.project);
        $http.post(host + '/Project/Build', data, config).then(successCallback, errorCallback);
    }
       
    $scope.deploy = function () {
        $scope.message = 'Processing deploy';
        prompt("Enter the port number", "4444").then(
            function (response) {
                $scope.loader = true;
                $scope.port = response;
                var data = new String($scope.project + ":" + $scope.port);
                $http.post(host_iis + '/Server', data, config).then(successCallback, errorCallback);
            }
        );
    }

    function successCallback(res) {
        //$scope.loader = false;
        //alert(res.data);
        $scope.message = res.data;
    }

});

//*************************************EntityCtrl******************************
app.controller('EntityCtrl', function ($scope, $http, $uibModal, $uibModalInstance, FileSaver, param) {

    if (param != undefined){
        if (param.project != undefined) $scope.project = param.project;
    }

    $scope.loader = false;
    $scope.options = {
        "mode": "code",
        "modes": ['tree', 'form', 'code', 'text'],
        "history": false
    };

    $scope.enableDB = true;


    var uri = host + '/Entity';

    $scope.showProjects = function () {
        var modalInstance = $uibModal.open({
            templateUrl: "choose-project.html",
            size: "sm",
            controller: "ProjectCtrl",
            resolve: {
                param: undefined
            }
        });

        //On -> Entity Project Selected
        modalInstance.result.then(function (project) {
            $scope.project = project;
        });
    };

    $scope.projects = [];

    $scope.getProjects = function () {
        $http.get(host + '/Project', config).then(getProjectsSuccess, errorCallback);
    }

    function getProjectsSuccess(res) {
        $scope.projects = res.data;
    }


    $scope.createEntity = function () {
        var data = {};
        data.Name = $scope.entityName || 'Global';

        if ($scope.enableDB) {
            //$scope.json.id = 0;
            data.enableDB = true;
        }
        data.Json = JSON.stringify($scope.json);
        data.Project = $scope.project;

        $scope.loader = true;
        $http.post(uri, data, config).then(entityCreated, errorCallback);
    }

    function entityCreated(res) {
        $scope.loader = false;
        alert(res.data);
        $uibModalInstance.dismiss();
    }
    
    //Browse json
    angular.extend($scope, {
        readMethod: "readAsText",
        onReaded: function (e, file) {
            $scope.json = JSON.parse(e.target.result);
        }
    });

    //Download json
    $scope.download = function () {
        var blob = new Blob([JSON.stringify($scope.json)], { type: 'application/json;charset=utf-8' });
        FileSaver.saveAs(blob, ($scope.entityName || 'document') + '.json');
    }

    //****Service -> Project -> Entity****
    $scope.getEntities = function (project) {
        if (project == undefined) project = 'Global';
        $scope.project = project;
        $http.get(uri + "?project=" + project , config).then(getEntitiesSuccess, errorCallback);
    }

    function getEntitiesSuccess(data) {
        $scope.entities = [];
        angular.forEach(data.data, function (value) {
            $scope.entities.push(value.Name);
        });
    }

    //****Entity->Project**** 
    $scope.choseEntity = {};

    $scope.entitySelected = function (entity) {
        param.entity = entity;
        param.project = $scope.project;
        $uibModalInstance.close(param);
    }

});

//*************************************ServiceCtrl******************************
app.controller('ServiceCtrl', function ($scope, $http, $uibModal, $uibModalInstance, $filter, $log, param) {

    if (param != undefined) {
        if (param.project != undefined) $scope.project = param.project;
    }

    $scope.loader = false;
    $scope.service = {
        "name": "",
        "project": "",
        "methods": [
          {
              "name": "Get",
              "enabled": false,
              "index":0,
              "id": "",
              "type": 0,
              "request": {
                  "uriParameters": [],
                  "headers": [],
                  "bodyParameters": [],
                  "sampleJson": ""
              },
              "response": {
                  "headers": [],
                  "bodyParameters": [],
                  "type": ""
              },
              "path": ""
          },
          {
              "name": "Post",
              "enabled": false,
              "index": 1,
              "id": "",
              "type": 0,
              "request": {
                  "uriParameters": [],
                  "headers": [],
                  "bodyParameters": [],
                  "sampleJson": ""
              },
              "response": {
                  "headers": [],
                  "bodyParameters": [],
                  "type": ""
              },
              "path": ""
          },
          {
              "name": "Put",
              "enabled": false,
              "index": 2,
              "id": "",
              "type": 0,
              "request": {
                  "uriParameters": [],
                  "headers": [],
                  "bodyParameters": [],
                  "sampleJson": ""
              },
              "response": {
                  "headers": [],
                  "bodyParameters": [],
                  "type": ""
              },
              "path": ""
          },
          {
              "name": "Delete",
              "enabled": false,
              "index": 3,
              "id": "",
              "type": 0,
              "request": {
                  "uriParameters": [],
                  "headers": [],
                  "bodyParameters": [],
                  "sampleJson": ""
              },
              "response": {
                  "headers": [],
                  "bodyParameters": [],
                  "type": ""
              },
              "path": ""
          }

        ]
    };

    var uri = host + '/Service';

    $scope.generateCode = function () {

        $scope.service.project = $scope.project || 'Global';
        
        for (var i = 0 ; i < $scope.service.methods.length; i++) {
            var method = $scope.service.methods[i];
            if ((method.name == "Post" || method.name == "Put") && method.enabled) {
                $scope.service.methods[i].request.bodyParameters.push({ name: $scope.service.methods[i].bodyParam, type: $scope.service.methods[i].bodyType })
            }
        };

        var data = $scope.service;
        var methods = $filter('filter')($scope.service.methods, { enabled: true });
        data.methods = methods;

        $scope.loaderSub = true;
        $http.post(uri, data, config).then(generateCodeSuccess, errorCallback);
    }

    function generateCodeSuccess(res) {
        $scope.loaderSub = false;
        if (res.data.indexOf('Error') == -1){
            $scope.code = res.data;
            alert('Code generated, View->Edit->Save');
        }
        else {
            alert(res.data);
            $scope.code = '';
        }
    }

    $scope.cancel = function () {
        $uibModal.dismiss();
    };
        

    $scope.showProjects = function () {
        var modalInstance = $uibModal.open({
            templateUrl: "choose-project.html",
            size: "sm",
            controller: "ProjectCtrl",
            resolve: {
                param: undefined
            }
        });

        //On -> service project selected
        modalInstance.result.then(function(project) {
            $scope.project = project;
        });
    };

    $scope.showEntities = function (index, response) {
        var modalInstance = $uibModal.open({
            templateUrl: "choose-entity.html",
            size: "sm",
            controller: "EntityCtrl",
            resolve: {
                param: {'index':index, 'response':response}
            }
        });

        //On -> service entity selected
        modalInstance.result.then(function (data) {            
            if(data.response=='uri')
                $scope.service.methods[data.index].uriType = data.entity;
            else if(data.response=='body')
                $scope.service.methods[data.index].bodyType = data.entity;
            else if (data.response == 'response')
                $scope.service.methods[data.index].response.type = data.entity;
        });
    };


    $scope.addReqParam = function (index) {
        $scope.service.methods[index].request.uriParameters.push({ name: $scope.service.methods[index].uriParam, type: $scope.service.methods[index].uriType })
        $scope.service.methods[index].uriParam = undefined;
        $scope.service.methods[index].uriType = 'string';
    }

    $scope.removeReqParam = function (index, modelIndex) {
        $scope.service.methods[modelIndex].request.uriParameters.splice(index, 1);
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
        $scope.service.project = $scope.project || 'Global';
        $scope.service.content = $scope.code;
        var data = $scope.service;
        data.methods = {};

        $scope.loader = true;
        $http.post(uri + '/Create', data, config).then(createServiceSuccess, errorCallback);
    }

    function createServiceSuccess(res) {
        $scope.loader = false;
        alert(res.data);
        $uibModalInstance.dismiss();
    }

    $scope.selectMethod = function (index) {
        if ($scope.service.methods[index].enabled) {
            angular.forEach($scope.service.methods, function (value) {
                value.active = false;
            });
            $scope.service.methods[index].active = true;
        }
    }
});


//*************************************FileCtrl*********************************
app.controller('FileCtrl', function ($scope, $http, $uibModal, $uibModalInstance, param) {

    $scope.loader = false;
    //param flow is from Service -> Code Viewer
    if (param != undefined) {

        if (param.code != undefined) $scope.fileViewer = param.code;
        if (param.uri != undefined) {
            $scope.uri = param.uri;
            $scope.title = getTitle(param.uri);
        }
        if (param.service != undefined) $scope.service = param.service;
        if (param.name != undefined) $scope.name = param.name;
        if (param.project != undefined) $scope.project = param.project;

        if (param.name == 'utility') {
            getUtilityTemplate($scope.project);
            $scope.title = param.title;
        }
    }
    else {
        $scope.title = 'File explorer';
        $scope.service = host + '/file';
        $scope.fileViewer = 'Please select a file to view its contents';
    }

    //Tree View
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
    

    //Ace
    $scope.mode = getMode($scope.uri);

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
        if (file == undefined) return 'csharp';

        var fileExt = file.split('.').pop();
        if (fileExt.toLowerCase() == 'js') return 'javascript';
        if (fileExt.toLowerCase() == 'cs') return 'csharp';
        if (fileExt.toLowerCase() == 'config' || fileExt.toLowerCase() == 'csproj' || fileExt.toLowerCase() == 'pubxml') return 'xml';
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

        modalInstance.result.then(function (project) {
            $scope.project = project;
        });
    };

    $scope.projects = [];

    $scope.getProjects = function () {
        var uri = host + '/Project';
        $http.get(uri, config).then(getProjectsSuccess, errorCallback);
    }

    function getProjectsSuccess(res) {
        $scope.projects = res.data;
    }

    function getUtilityTemplate(project) {
        var uri = host + '/Utility';
        if (project != undefined) uri = uri + '?project=' + project;

        $http.get(uri).then(getUtilityTemplateSuccess, errorCallback);
    }

    function getUtilityTemplateSuccess(res) {
        $scope.fileViewer = res.data;
    }

    $scope.createFile = function () {
        var data = {};
        data.Name = $scope.fileName;
        data.Content = $scope.fileViewer;
        data.Project = $scope.project;

        $scope.loader = true;
        $http.post(host + '/Utility', data, config).then(createFileSuccess, errorCallback);
    }

    function createFileSuccess(res) {
        $scope.loader = false;
        alert(res.data);
        $uibModalInstance.dismiss();
    }

    $scope.save = function (uri) {
        if (uri != undefined) {
            var data = new String($scope.fileViewer);
            $http.post(host + '/File/Save?file=' + uri, data, config).then(createFileSuccess, errorCallback);
        }
        else
        $uibModalInstance.close($scope.fileViewer);
    }
});

//*************************************RestCtrl******************************
app.controller('RestCtrl', function ($scope, $http, $uibModal, $uibModalInstance) {
    $scope.method = 'GET';
    $scope.options = {
        "mode": 'code',
        "modes": ['tree', 'form', 'code', 'text'],
        "history": false
    };
    $scope.result = false;
    $scope.aceOption = {
        mode: 'json',
        onLoad: function (_ace) {
            //The ace instance in the scope...
            $scope.modeChanged = function () {
                _ace.getSession().setMode("ace/mode/" + 'json');
            };

        }
    };
    $scope.headers = config.headers;
    $scope.uri = host;
    
    $scope.req = { data : "{}" };
    $scope.res = "Response";
    $scope.obj = { 
        headerKey : '',
        headerVal : ''
    }
    $scope.addHeader = function () {
        $scope.headers[$scope.obj.headerKey] = $scope.obj.headerVal;
        $scope.obj.headerKey = '';
        $scope.obj.headerVal = '';
    }
    $scope.removeHeader = function (key) {
        delete $scope.headers[key];
    }
    $scope.send = function () {
        var method = $scope.method;
        var uri = $scope.uri;

        var config = { headers: $scope.headers };
        var data = $scope.enableString ? (new String($scope.dataString)) : JSON.stringify($scope.req.data);
        $scope.loader = true;
        switch (method) {
            case 'GET':
                $http.get(uri,config).then(restSuccess,restError);
                break;
            case 'POST':                
                $http.post(uri,data,config).then(restSuccess,restError);
                break;
            case 'PUT':
                $http.put(uri,data,config).then(restSuccess,restError);
                break;
            case 'DELETE':
                $http.delete(uri,data,config).then(restSuccess,restError);
                break;
        }
    }
    function restSuccess(res) {
        alert('Received response');
        $scope.loader = false;
        $scope.res = JSON.stringify(res, null, 2);
        $scope.result = true;

    }
    function restError(res) {
        $scope.res = JSON.stringify(res, null, 2);
        $scope.result = true;
    }
});

//*************************************Common***********************************
var config = {
    headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
    }
};

function errorCallback(res) {
    if (res.data.Message != undefined)
        alert(res.data.Message);
    else if (res.statusText != undefined)
        alert(res.statusText);
    else
        alert('Error occured');
}

function getTitle(url) {
    if (url.indexOf('project-repository') != -1)
        return 'Code Viewer' + url.split('project-repository')[1].replace(/\\/g, '>');
    else if (url.indexOf('builds') != -1)
        return 'Code Viewer' + url.split('builds')[1].replace(/\\/g, '>');
    else return 'Code Viewer' + url;
}