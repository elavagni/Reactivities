import { observer } from 'mobx-react-lite';
import React, { useEffect, useState } from 'react';
import InfiniteScroll from 'react-infinite-scroller';
import { Grid, Loader } from 'semantic-ui-react';
import LoadingComponent from '../../../app/layout/LoadingComponent';
import { PagingParams } from '../../../app/models/pagination';
import { useStore } from '../../../app/stores/store';
import ActivityFilters from './ActivityFilters';
import ActivityList from './ActivityList';

export default observer(function ActivityDashboard() {
  const { activityStore } = useStore();
  const { loadActivities, activityRegistry, setPagingParams, pagination } = activityStore;
  const [loadingNextPage, setLoadingNextPage] = useState(false);

  async function handleGetNextPage() {
    setLoadingNextPage(true);
    setPagingParams(new PagingParams(pagination!.currentPage + 1, pagination?.itemsPerPage));
    await loadActivities();
    setLoadingNextPage(false);
  }

  useEffect(() => {
    if (activityRegistry.size <= 1) loadActivities();
  }, [activityRegistry.size, loadActivities]);

  //TODO: rename loadingInitial
  //do not show the loading indicator if we are loading the next page of results
  if (activityStore.loadingInitial && !loadingNextPage)
    return <LoadingComponent content="Loading activities..." />;

  return (
    <Grid>
      <Grid.Column width="10">
        <InfiniteScroll
          pageStart={0}
          loadMore={handleGetNextPage}
          hasMore={
            !loadingNextPage && !!pagination && pagination.currentPage < pagination.totalPages
          }
          initialLoad={false}
        >
          <ActivityList />
        </InfiniteScroll>
      </Grid.Column>
      <Grid.Column width="6">
        <ActivityFilters />
      </Grid.Column>
      <Grid.Column width="10">
        <Loader active={loadingNextPage} />
      </Grid.Column>
    </Grid>
  );
});
