//TESTER = document.getElementById('tester');
//Plotly.plot(TESTER, [{
//    x: [1, 2, 3, 4, 5],
//    y: [1, 2, 4, 8, 16]
//}], {
//        margin: { t: 0 }
//    });

var trace1 = {
    x: [0, 1, 2, 3, 4, 5, 6, 7, 8],
    y: [0, 1, 2, 3, 4, 5, 6, 7, 8],
    name: 'Name of Trace 1',
    type: 'scatter'
};
var trace2 = {
    x: [0, 1, 2, 3, 4, 5, 6, 7, 8],
    y: [1, 0, 3, 2, 5, 4, 7, 6, 8],
    name: 'Name of Trace 2',
    type: 'scatter'
};

var data = [trace1, trace2];
var layout = {
    title: 'Plot Title',
    xaxis: {
        title: 'x Axis',
        titlefont: {
            family: 'Courier New, monospace',
            size: 18,
            color: '#7f7f7f'
        }
    },
    yaxis: {
        title: 'y Axis',
        titlefont: {
            family: 'Courier New, monospace',
            size: 18,
            color: '#7f7f7f'
        }
    }
};

function drawPlot(data1)
{
    data1 = JSON.parse(data1);
    var layout = {
        title: data1.title,
        xaxis: {
            title: 'x Axis',
            titlefont: {
                family: 'Courier New, monospace',
                size: 18,
                color: '#7f7f7f'
            }
        },
        yaxis: {
            title: 'y Axis',
            titlefont: {
                family: 'Courier New, monospace',
                size: 18,
                color: '#7f7f7f'
            }
        }
    };
    data = [data1];
    Plotly.newPlot('tester', data, layout);
}



