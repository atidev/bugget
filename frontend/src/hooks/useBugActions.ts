import { useUnit } from "effector-react";
import {
  addBugEvent,
  updateBugDataEvent,
  changeBugStatusEvent,
  getBugsForReport,
} from "@/store/bugs";
import { $reportIdStore } from "@/store/report";
import { BugStatuses } from "@/const";
import { BugFormData } from "@/types/bug";

export const useBugActions = () => {
  const reportId = useUnit($reportIdStore);
  const bugs = useUnit(getBugsForReport(reportId));

  const createBug = (data: BugFormData) => {
    if (!reportId) return;
    addBugEvent({ reportId, data });
  };

  const updateBugFields = (bugId: number, data: Partial<BugFormData>) => {
    if (!reportId) return;
    updateBugDataEvent({ bugId, reportId, data });
  };

  const changeBugStatus = (bugId: number, status: BugStatuses) => {
    changeBugStatusEvent({ bugId, status });
  };

  const updateReceive = (bugId: number, receive: string) => {
    updateBugFields(bugId, { receive });
  };

  const updateExpect = (bugId: number, expect: string) => {
    updateBugFields(bugId, { expect });
  };

  return {
    reportId,
    bugs,

    createBug,
    updateBugFields,
    changeBugStatus,
    updateReceive,
    updateExpect,
  };
};
