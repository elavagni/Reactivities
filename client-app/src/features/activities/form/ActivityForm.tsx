import { observer } from 'mobx-react-lite';
import React, { ChangeEvent, useEffect, useState } from 'react';
import { Link, useHistory, useParams } from 'react-router-dom';
import { Button, Form, Segment } from 'semantic-ui-react';
import LoadingComponent from '../../../app/layout/LoadingComponent';
import { useStore } from '../../../app/stores/store';
import { v4 as uuid } from 'uuid';
import { Formik } from 'formik';

export default observer(function ActivityForm() {
  const history = useHistory();
  const { activityStore } = useStore();
  const { createActivity, updateActivity, loading, loadActivity, loadingInitial } = activityStore;
  const { id } = useParams<{ id: string }>();

  const [activity, setActivity] = useState({
    id: '',
    title: '',
    category: '',
    description: '',
    date: '',
    city: '',
    venue: '',
    isCancelled: false,
  });

  useEffect(() => {
    if (id) loadActivity(id).then((activity) => setActivity(activity!));
  }, [id, loadActivity]);

  // async function handleSubmit() {
  //   if (activity.id.length === 0) {
  //     let newActivity = {
  //       ...activity,
  //       id: uuid(),
  //     };
  //     await createActivity(newActivity);
  //     history.push(`/activities/${newActivity.id}`);
  //   } else {
  //     await updateActivity(activity);
  //     history.push(`/activities/${activity.id}`);
  //   }
  // }

  // function handleInputChange(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) {
  //   const { name, value } = event.target;
  //   setActivity({ ...activity, [name]: value });
  // }

  if (loadingInitial) return <LoadingComponent content="Loading activity..." />;

  return (
    <Formik enableReinitialize initialValues={activity} onSubmit={(values) => console.log(values)}>
      {({ values: activity, handleChange, handleSubmit }) => (
        <Segment clearing>
          <Form onSubmit={handleSubmit} autoComplete="off">
            <Form.Input
              placeholder="Title"
              value={activity.title}
              name="title"
              onChange={handleChange}
            />
            <Form.TextArea
              placeholder="Description"
              value={activity.description}
              name="description"
              onChange={handleChange}
            />
            <Form.Input
              placeholder="Category"
              value={activity.category}
              name="category"
              onChange={handleChange}
            />
            <Form.Input
              type="date"
              placeholder="Date"
              value={activity.date}
              name="date"
              onChange={handleChange}
            />
            <Form.Input
              placeholder="City"
              value={activity.city}
              name="city"
              onChange={handleChange}
            />
            <Form.Input
              placeholder="Venue"
              value={activity.venue}
              name="venue"
              onChange={handleChange}
            />
            <Button loading={loading} floated="right" positive type="submit" content="Submit" />
            <Button as={Link} to="/activities" floated="left" type="button" content="Cancel" />
          </Form>
        </Segment>
      )}
    </Formik>
  );
});
