var express = require('express');
var app = express();

app.get('/', function(req, res){
	res.send('hello world!');
});

app.post('/', function(req, res){
	res.send('posted');
});

app.put('/', function(req, res){
	res.send('put');
});

app.delete('/', function(req, res){
	res.send('del');
});



var server = app.listen(3000, function(){
	var host = server.address().address;
	var port = server.address().port;

	console.log('listening @ http://%s:%s', host, port);
});