% close all
clear
mkBold(1)

% Develoments of a KDTree algorithm

%% Parameters
nPts = 50; %50; % Total number of points
dims = 2; % Number of dimensions
scaleLim = 100; % Random point limits
seed = 123; % RNG seed

Y = [40, 60]; % Search point
searchCnt = 3; % Number of nearest neighbors
searchRng = 10; % Search range

bInf = [0, scaleLim]; % Bounds infinite values to these numbers

savePre = 'KDtree_Ex_';
figDir = 'Figs_KDtree_Ex_';

%% Prep
dircheck(figDir)

%% .NET Prep
% Adds Isidore namespace assemblies
% loads assemblies into MatLab (You must use the full path)
warning off MATLAB:NET:AddAssembly:nameConflict
nameSpaces = {'KDTree'};
assName = {'kdtree'};
for idx = 1:length(nameSpaces)
    %fullPath = which([nameSpaces{idx}, '.dll']);
    fullPath = which([nameSpaces{idx}, '.dll']);
    try
        eval([assName{idx}, ' = NET.addAssembly(fullPath);']);
        eval(['import ', nameSpaces{idx},'.*;']);
    catch e
        e.message
        if(isa(e, 'NET.NetException'))
            e.ExceptionObject
        end
    end
end

% Assorted Property info for user reference
kdtreeClasses = kdtree.Classes;
% These provide the function sigatures.
% Since constructors are technically methods, they are also listed
kdTreeMeths = methods('KDTree','-full');

%% Random data set
rng(seed);
X = scaleLim * rand(nPts, dims);

%% K-D tree generation
% Assembles points into a list of points
ptVec = NET.createGeneric('System.Collections.Generic.List', ...
    {'KDTree.Point'});
for idx = 1:nPts
    ptVec.Add(KDTree.Point(X(idx, 1), X(idx, 2)));
end

% K-D Tree initialization
kdTree = KDTree.KDTree(ptVec);

%% Extracts box-node tree
len = kdTree.boxes.Length;
corn = zeros(len, 2);
wide = corn;
for idx = 1:len
    % box points
    low = double(kdTree.boxes(idx).lo.Comp);
    hi = double(kdTree.boxes(idx).hi.Comp);
    % limits extends of border boxes
    low = max(0, low);
    hi = min(scaleLim, hi);
    % cast in form in rectangle
    corn(idx, :) = low(1:2);
    wide(idx, :) = hi(1:2) - low(1:2);
end

%% Nearest neighbors
% Search Point
searchPt = KDTree.Point(Y(1), Y(2), 0);

% Nearest neighbor
nn = kdTree.Nearest(searchPt);
nnIdx = nn.Item1 + 1; % Corrects for 0 vs. 1 index
nnDist = nn.Item2;

% Range search
rs = kdTree.LocateNear(searchPt, searchRng);
rsIdx = int32(rs.Item1) + 1;
rsDist = double(rs.Item2);

% Nearest three points
rs3 = kdTree.LocateNear(searchPt, searchRng, 3);
rs3Idx = int32(rs3.Item1) + 1;
rs3Dist = double(rs3.Item2);

%% Builds Conventional MatLab KD-Tree Structure
% Euclidean distance, 2 points per bucket
Md = KDTreeSearcher(X, 'BucketSize', 2); 

%% Plotter
% Scatter plot
plot(X(:,1), X(:,2), '*');
for idx = 1:size(corn, 1)
    rectangle('Position', [corn(idx, :), wide(idx,:)]);
end
axis image
ffile = fullfile(figDir, [savePre, 'Nodes']);
printFig(ffile)

% Search comparision
figure
hold on
    hall = plot(X(:,1), X(:,2), '*');
    hpt = plot(Y(:,1), Y(:,2), 'p');
    hrs = plot(X(rsIdx,1), X(rsIdx,2), 'o');
    hrs3 = plot(X(rs3Idx,1), X(rs3Idx,2), '+');
    hnn = plot(X(nnIdx,1), X(nnIdx,2), 'x');
    box on
hold off
hallAnno = get(hall, 'Annotation');
hallLegend = get(hallAnno, 'LegendInformation');
set(hallLegend, 'IconDisplayStyle', 'off');
axis image
nnStr = 'Nearest';
rsStr = sprintf('Range < %d', searchRng);
% rs3Str = sprintf('Nearest %u & Range < %d', searchCnt, searchRng);
rs3Str = sprintf('Nearest %u', searchCnt);
h = legend('Query Pt', rsStr, rs3Str, nnStr, 'Location', 'NE');
set(h, 'color', 'none');
xlim([0,100])
ylim([0,100])
ffile = fullfile(figDir, [savePre, 'Comp']);
printFig(ffile)

%% Clean-up
old = cd(figDir);
RemoveWhiteBorder;
cd(old)

mkBold(0)