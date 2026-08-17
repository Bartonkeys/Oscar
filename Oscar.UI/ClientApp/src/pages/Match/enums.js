import { get } from "../../shared/helpers/apiaccess"

export const getStatuses = async () => {
  return await get ('/staticData/client/statuses');
};

export const getGrades = async () => {
  return await get ('/staticData/client/grades');
};