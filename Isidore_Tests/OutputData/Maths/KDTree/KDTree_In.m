%% Parameters
nPts = 50; %50; % Total number of points
dims = 2; % Number of dimensions
scaleLim = 100; % Random point limits
seed = 123; % RNG seed

Y = [40, 60]; % Search point
searchCnt = 3; % Number of nearest neighbors
searchRng = 10; % Search range

bInf = [0, scaleLim]; % Bounds infinite values to these numbers

%% Random data set
rng(seed);
X = scaleLim * rand(nPts, dims);