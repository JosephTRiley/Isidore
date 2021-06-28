% Data pushed by OctreeTraceCheck.cs
% posArray = All octree AABB center points
% ubtArray = All octure AABB index descriptors


% close all; % So it can be run from MatLab

%% Process indices
tag = idxArray ~= -1;
treeDepth = sum(tag,2);

%% Displays data
cVal = [0,0,0.75; 0.75,0,0; 0,0.75,0; 0.75,0.75,0];
figure; 
for idx = 0:(max(treeDepth)-1)
    tag = treeDepth == (idx + 1);
    cIdx = 1 + mod(idx, size(cVal,1));
    hold on
        plot3(posArray(tag, 1), posArray(tag, 2), posArray(tag, 3), ...
            '.', 'Color', cVal(cIdx, :));
    hold off
end
camorbit(45, -45)
legend('Rank 1', 'Rank 2', 'Rank 3', 'Rank 4')
%% Saves
save('OctreeTrace')