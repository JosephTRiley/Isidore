mkBold(1)

%% Folder check

savePre = 'KDtree_Ex_';
figDir = 'Figs_KDtree_Ex';

dircheck(figDir)

%% Switches index from .NET to MatLab
nnIdx = nnIdx + 1;
rsIdx = rsIdx + 1;
rs3Idx = rs3Idx + 1;

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