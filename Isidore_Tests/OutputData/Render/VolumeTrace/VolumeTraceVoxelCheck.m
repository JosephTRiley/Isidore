% close all; % So I can run it from MatLab

%% Displays data
figure; 

subplot(2,3,1)
imagesc(pos0, pos1, intCnt0); 
axis image; axis xy; colorbar; 
ylabel('Projector 0')
title('Intersect')

subplot(2,3,2) 
imagesc(pos0, pos1, surfID0); 
axis image; axis xy; colorbar; 
title('Surf Depth')

subplot(2,3,3)
imagesc(pos0, pos1, volume0); 
axis image; axis xy; colorbar; 
title('Vol. Depth')

subplot(2,3,4) 
imagesc(pos0, pos1, intCnt1); 
axis image; axis xy; colorbar;
ylabel('Projector 1')

subplot(2,3,5)
imagesc(pos0, pos1, surfID1); 
axis image; axis xy; colorbar; 

subplot(2,3,6) 
imagesc(pos0, pos1, volume1); 
axis image; colorbar; 

%% Saves
save('VolumeTraceVoxel')